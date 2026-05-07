declare module 'luxon' {
    export class DateTime {
      static fromISO(iso: string): DateTime;
      toISO(): string;
      // Add any other relevant methods you use from luxon
    }
  }
  