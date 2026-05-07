// nav-item/nav-item.ts
export interface NavItem {
  navCap?: string;
  displayName?: string;
  iconName?: string;
  route?: string;
  divider?: boolean;
  roles?: string[]; // Make sure this exists
}