import {
  AfterViewChecked,
  ChangeDetectorRef,
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { catchError, finalize, forkJoin, of, Subject, takeUntil } from 'rxjs';

import { Account } from 'src/app/Models/account.model';
import { Message } from 'src/app/Models/message.model';
import { AccountService } from 'src/app/services/account.service';
import { MessageService } from 'src/app/services/message.service';

@Component({
  selector: 'app-message-modal',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatSnackBarModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    MatProgressSpinnerModule,
    MatFormFieldModule
  ],
  templateUrl: './message-modal.component.html',
  styleUrls: ['./message-modal.component.css'],
})
export class MessagePageComponent implements OnInit, OnDestroy, AfterViewChecked {
  accounts: Account[] = [];
  selectedReceiver: Account | null = null;
  messages: Message[] = [];
  newMessage = '';
  contactsLoading = false;
  messagesLoading = false;
  isSending = false;
  searchQuery = '';

  private readonly senderId = Number(localStorage.getItem('accountId')) || 0;
  private readonly conversationsByAccount = new Map<number, Message[]>();
  private readonly destroy$ = new Subject<void>();
  private activeConversationId: number | null = null;
  private shouldScrollToBottom = false;
  private isDestroyed = false;

  @ViewChild('messageContainer') private messageContainer?: ElementRef<HTMLElement>;
  @ViewChild('messageInput') private messageInput?: ElementRef<HTMLInputElement>;

  constructor(
    private accountService: AccountService,
    private messageService: MessageService,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    if (!this.senderId) {
      this.snackBar.open('Cannot open messages: current account is missing.', 'Close', { duration: 4000 });
      return;
    }

    this.loadAccounts();
  }

  ngOnDestroy(): void {
    this.isDestroyed = true;
    this.destroy$.next();
    this.destroy$.complete();
  }

  ngAfterViewChecked(): void {
    if (this.shouldScrollToBottom) {
      this.scrollToBottom();
      this.shouldScrollToBottom = false;
    }
  }

  loadAccounts(): void {
    if (!this.senderId) return;

    this.contactsLoading = true;
    this.refreshView();

    this.accountService.getAccounts()
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => {
          this.contactsLoading = false;
          this.refreshView();
        })
      )
      .subscribe({
        next: (accounts) => {
          this.accounts = accounts.filter(account => account.id !== this.senderId);
          this.loadConversationSummaries();

          if (!this.selectedReceiver && this.accounts.length > 0) {
            this.selectReceiver(this.accounts[0]);
            return;
          }

          if (this.selectedReceiver && !this.accounts.some(account => account.id === this.selectedReceiver?.id)) {
            this.selectedReceiver = this.accounts[0] ?? null;
            this.messages = [];

            if (this.selectedReceiver) {
              this.selectReceiver(this.selectedReceiver);
            }
          }

          this.refreshView();
        },
        error: (error) => {
          console.error('Failed to load contacts:', error);
          this.snackBar.open('Failed to load contacts', 'Close', { duration: 3000 });
        }
      });
  }

  selectReceiver(account: Account): void {
    this.selectedReceiver = account;
    this.messages = this.getConversationMessages(account.id);
    this.shouldScrollToBottom = true;
    this.refreshView();
    this.loadMessages(account.id);
    this.focusInput();
  }

  loadMessages(receiverId = this.selectedReceiver?.id): void {
    if (!this.senderId || !receiverId) return;

    this.activeConversationId = receiverId;
    this.messagesLoading = true;
    this.refreshView();

    this.messageService.getConversation(this.senderId, receiverId)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => {
          if (this.activeConversationId === receiverId) {
            this.messagesLoading = false;
            this.refreshView();
          }
        })
      )
      .subscribe({
        next: (messages) => {
          const sortedMessages = this.sortMessages(messages);
          this.setConversationMessages(receiverId, sortedMessages);

          if (this.selectedReceiver?.id === receiverId) {
            this.messages = sortedMessages;
            this.markIncomingMessagesAsRead(receiverId, sortedMessages);
            this.shouldScrollToBottom = true;
          }

          this.refreshView();
        },
        error: (error) => {
          console.error('Failed to load messages:', error);
          this.snackBar.open('Failed to load messages', 'Close', { duration: 3000 });
        }
      });
  }

  sendMessage(): void {
    const content = this.newMessage.trim();
    const receiverId = this.selectedReceiver?.id;

    if (!content || !receiverId || this.isSending || this.messagesLoading) return;
    if (!this.senderId) {
      this.snackBar.open('Cannot send message: current account is missing.', 'Close', { duration: 4000 });
      return;
    }

    this.isSending = true;
    this.refreshView();

    this.messageService.sendMessage(this.senderId, receiverId, content)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => {
          this.isSending = false;
          this.refreshView();
        })
      )
      .subscribe({
        next: (message) => {
          const conversation = this.sortMessages([
            ...this.getConversationMessages(receiverId),
            message
          ]);

          this.setConversationMessages(receiverId, conversation);

          if (this.selectedReceiver?.id === receiverId) {
            this.messages = conversation;
          }

          this.newMessage = '';
          this.shouldScrollToBottom = true;
          this.focusInput();
          this.refreshView();
        },
        error: (error) => {
          console.error('Error sending message:', error);
          this.snackBar.open('Failed to send message', 'Close', { duration: 5000 });
        }
      });
  }

  isSentByCurrentUser(message: Message): boolean {
    return message.senderId === this.senderId;
  }

  getMessageTime(timestamp: string | Date | undefined): string {
    if (!timestamp) return '';

    const date = this.toDate(timestamp);
    return date ? date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }) : '';
  }

  getInitials(name: string): string {
    if (!name) return '';

    return name
      .split(' ')
      .filter(Boolean)
      .map(part => part[0])
      .join('')
      .slice(0, 2)
      .toUpperCase();
  }

  hasUnreadMessages(accountId: number): boolean {
    return this.getConversationMessages(accountId).some(message =>
      message.receiverId === this.senderId &&
      message.senderId === accountId &&
      !message.isRead
    );
  }

  shouldShowDate(index: number): boolean {
    if (index === 0) return true;

    const currentMessage = this.messages[index];
    const previousMessage = this.messages[index - 1];
    const currentDate = this.toDate(currentMessage?.timestamp)?.toDateString();
    const previousDate = this.toDate(previousMessage?.timestamp)?.toDateString();

    return !!currentDate && currentDate !== previousDate;
  }

  getMessageDate(timestamp: string | Date | undefined): string {
    const date = this.toDate(timestamp);
    if (!date) return '';

    return date.toLocaleDateString([], {
      weekday: 'long',
      month: 'short',
      day: 'numeric'
    });
  }

  get filteredAccounts(): Account[] {
    const query = this.searchQuery.trim().toLowerCase();
    if (!query) return this.accounts;

    return this.accounts.filter(account =>
      account.name.toLowerCase().includes(query) ||
      account.username.toLowerCase().includes(query) ||
      account.role.toLowerCase().includes(query)
    );
  }

  getLastMessageTime(accountId: number): string | null {
    const lastMessage = this.getLastMessage(accountId);
    return lastMessage?.timestamp ? this.getMessageTime(lastMessage.timestamp) : null;
  }

  getLastMessagePreview(accountId: number): string | null {
    const lastMessage = this.getLastMessage(accountId);
    if (!lastMessage?.content) return null;

    return this.isSentByCurrentUser(lastMessage)
      ? `You: ${lastMessage.content}`
      : lastMessage.content;
  }

  getTruncatedPreview(accountId: number): string {
    const preview = this.getLastMessagePreview(accountId);
    if (!preview) return '';

    return preview.length > 38 ? `${preview.substring(0, 38)}...` : preview;
  }

  private loadConversationSummaries(): void {
    if (!this.senderId || this.accounts.length === 0) return;

    const requests = this.accounts.map(account =>
      this.messageService.getConversation(this.senderId, account.id).pipe(
        catchError((error) => {
          console.warn(`Failed to load conversation summary for account ${account.id}:`, error);
          return of([] as Message[]);
        })
      )
    );

    forkJoin(requests)
      .pipe(takeUntil(this.destroy$))
      .subscribe((conversations) => {
        this.accounts.forEach((account, index) => {
          this.setConversationMessages(account.id, conversations[index] ?? []);
        });

        if (this.selectedReceiver) {
          this.messages = this.getConversationMessages(this.selectedReceiver.id);
        }

        this.refreshView();
      });
  }

  private markIncomingMessagesAsRead(accountId: number, messages: Message[]): void {
    const unreadMessages = messages.filter(message =>
      message.id &&
      message.senderId === accountId &&
      message.receiverId === this.senderId &&
      !message.isRead
    );

    if (unreadMessages.length === 0) return;

    forkJoin(unreadMessages.map(message => this.messageService.markMessageAsRead(message.id!)))
      .pipe(
        takeUntil(this.destroy$),
        catchError((error) => {
          console.warn('Failed to mark messages as read:', error);
          return of([]);
        })
      )
      .subscribe(() => {
        unreadMessages.forEach(message => message.isRead = true);
        this.setConversationMessages(accountId, messages);
        this.refreshView();
      });
  }

  private getConversationMessages(accountId: number): Message[] {
    return this.conversationsByAccount.get(accountId) ?? [];
  }

  private setConversationMessages(accountId: number, messages: Message[]): void {
    this.conversationsByAccount.set(accountId, this.sortMessages(messages));
  }

  private getLastMessage(accountId: number): Message | undefined {
    const conversationMessages = this.getConversationMessages(accountId);
    return conversationMessages[conversationMessages.length - 1];
  }

  private sortMessages(messages: Message[]): Message[] {
    return [...messages].sort((a, b) => {
      const timeA = this.toDate(a.timestamp)?.getTime() ?? 0;
      const timeB = this.toDate(b.timestamp)?.getTime() ?? 0;
      return timeA - timeB;
    });
  }

  private toDate(timestamp: string | Date | undefined): Date | null {
    if (!timestamp) return null;

    const date = typeof timestamp === 'string' ? new Date(timestamp) : timestamp;
    return Number.isNaN(date.getTime()) ? null : date;
  }

  private scrollToBottom(): void {
    const container = this.messageContainer?.nativeElement;
    if (!container) return;

    container.scrollTop = container.scrollHeight;
  }

  private focusInput(): void {
    setTimeout(() => this.messageInput?.nativeElement.focus(), 0);
  }

  private refreshView(): void {
    if (!this.isDestroyed) {
      this.cdr.detectChanges();
    }
  }
}
