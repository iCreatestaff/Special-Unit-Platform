import { ChangeDetectorRef, Component, Inject, ViewChild, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AccountService } from 'src/app/services/account.service';
import { Account } from 'src/app/Models/account.model';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MaterialModule } from 'src/app/material.module';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-account-modal',
  templateUrl: './account-modal.component.html',
  standalone: true,
  imports: [MatTableModule, FormsModule, CommonModule, MaterialModule],
  styleUrls: ['./account-modal.component.css'],
})
export class AccountModalComponent implements OnInit {
  account: Account;
  password: string ='';
  imageUrl: string = '';
  fileNamePhoto: string | null = null;
  fileNameBadge: string | null = null;
  loading: boolean = false;
  loadingAccount: boolean = false;
  errorMessage: string = '';
  @ViewChild('fileUpload') fileUpload: any;
  @ViewChild('fileUpload1') fileUpload1: any;

  constructor(
    public dialogRef: MatDialogRef<AccountModalComponent>,
    private accountService: AccountService,
    private cdr: ChangeDetectorRef,
    @Inject(MAT_DIALOG_DATA) public data: { accountId: number } // Expecting accountId instead of account
  ) {
    // Initialize account to default values
    this.account = {
      id: 0,
      name: '',
      username: '',
      badge: '',
      role: '',
      type: '',
      photo: '',
    };
  }

  ngOnInit(): void {
    if (this.data?.accountId) {
      this.loadAccountData(this.data.accountId);
    }
  }

  get isEditMode(): boolean {
    return this.account.id !== 0;
  }

  checkRole(): string {
    const role = localStorage.getItem('role');
    return role || '';
  }

  loadAccountData(accountId: number): void {
    this.loadingAccount = true;
    this.errorMessage = '';
    this.accountService.getAccountById(accountId).subscribe({
      next: (account) => {
        this.account = account;
        this.password = '';
        this.imageUrl = account.photo || '';
        this.fileNamePhoto = account.photo ? 'Current photo' : null;
        this.fileNameBadge = account.badge ? 'Current badge' : null;
        this.loadingAccount = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load account:', err);
        this.errorMessage = 'Failed to load account details.';
        this.loadingAccount = false;
        this.cdr.detectChanges();
      }
    });
  }

  validateForm(): boolean {
    if (!this.account.name?.trim() || !this.account.username?.trim() || !this.account.role || !this.account.type) {
      alert('Please fill out all required fields.');
      return false;
    }
  
    if (!this.isEditMode && (!this.password?.trim() || this.password.length < 3)) {
      alert('Password is required and must be at least 3 characters long.');
      return false;
    }

    if (this.isEditMode && this.password?.trim() && this.password.length < 3) {
      alert('New password must be at least 3 characters long.');
      return false;
    }
  
    if (!this.isEditMode && !this.account.photo && !this.fileNamePhoto) {
      alert('Please upload a photo or provide a photo URL.');
      return false;
    }
  
    if (!this.isEditMode && !this.account.badge && !this.fileNameBadge) {
      alert('Please upload a badge or provide a badge URL.');
      return false;
    }
  
    return true;
  }
  

  onSave(): void {
    if (!this.validateForm()) {
      return;
    }
    this.loading = true;
    const accountData: Account = {
      ...this.account,
      password: this.password?.trim() || undefined,
    };
    if (this.account.id === 0) {
      this.createAccount(accountData);
    } else {
      this.updateAccount(accountData);
    }
  }

  createAccount(account: Account): void {
    if (account.role === 'Admin') {
      this.accountService.createAdmin(account).subscribe({
        next: (createdAccount) => {
          console.log('Admin account created successfully:', createdAccount);
          this.dialogRef.close(createdAccount);
        },
        error: (err) => {
          console.error('Failed to create admin account:', err);
          this.loading = false;
          alert('Failed to create admin account.');
        },
      });
    } else if (account.role === 'Agent') {
      this.accountService.createAgent(account).subscribe({
        next: (createdAccount) => {
          console.log('Agent account created successfully:', createdAccount);
          this.dialogRef.close(createdAccount);
        },
        error: (err) => {
          console.error('Failed to create agent account:', err);
          this.loading = false;
          alert('Failed to create agent account.');
        },
      });
    } else {
      console.error('Invalid role specified.');
      this.loading = false;
      alert('Invalid role specified.');
    }
  }

  updateAccount(account: Account): void {
    this.accountService.updateAccount(account.id, account).subscribe({
      next: (updatedAccount) => {
        console.log('Account updated successfully:', updatedAccount);
        this.dialogRef.close(updatedAccount);
      },
      error: (err) => {
        console.error('Failed to update account:', err);
        this.loading = false;
        alert('Failed to update account.');
      },
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  private async handleImageUpload(event: Event, isPhoto: boolean): Promise<void> {  
    const input = event.target as HTMLInputElement;  
    if (input.files && input.files.length > 0) {  
      const file = input.files[0];  

      // Validate file type  
      if (!file.type.startsWith('image/')) {  
        alert(`Please select a valid image file for ${isPhoto ? 'photo' : 'badge'}.`);  
        return;  
      }  

      // Maximum file size (e.g., 1MB)  
      const maxFileSize = 1024 * 1024; // 1MB  
      if (file.size > maxFileSize) {  
        alert(`File size exceeds the limit of 1MB.`);  
        return;  
      }  

      // Create a FileReader to read the selected file  
      const reader = new FileReader();  
      reader.onload = async (e: ProgressEvent<FileReader>) => {  
        const img = new Image();  
        img.onload = async () => {  
          const MAX_DIMENSION = 300; // Maximum dimension for resizing  
          let [width, height] = [img.width, img.height];  

          // Scale down if necessary  
          if (width > height) {  
            if (width > MAX_DIMENSION) {  
              height = (height * MAX_DIMENSION) / width;  
              width = MAX_DIMENSION;  
            }  
          } else {  
            if (height > MAX_DIMENSION) {  
              width = (width * MAX_DIMENSION) / height;  
              height = MAX_DIMENSION;  
            }  
          }  

          const canvas = document.createElement('canvas');  
          const ctx = canvas.getContext('2d');  

          if (ctx) {  
            canvas.width = width;  
            canvas.height = height;  
            ctx.clearRect(0, 0, canvas.width, canvas.height); // Clear the canvas for transparency  
            ctx.drawImage(img, 0, 0, width, height);  
            
            // Use lower quality for better compression  
            const compressedImageUrl = canvas.toDataURL('image/png'); // Adjust quality as needed  
            
            // Update account data  
            if (isPhoto) {  
              this.account.photo = compressedImageUrl;  
              this.fileNamePhoto = file.name;  
            } else {  
              this.account.badge = compressedImageUrl;  
              this.fileNameBadge = file.name;  
            }
            this.cdr.detectChanges();
          }  
        };  
        img.src = e.target?.result as string;  
      };  
      reader.readAsDataURL(file);  
    }  
  }  

  onPhotoFileSelected(event: Event): void {  
    this.handleImageUpload(event, true);  
  }  
  
  onBadgeFileSelected(event: Event): void {  
    this.handleImageUpload(event, false);  
  } 
  

  onUrlEntered(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.imageUrl = input.value;
    if (this.imageUrl) {
      this.account.photo = this.imageUrl;
    }
    this.cdr.detectChanges();
  }
}
