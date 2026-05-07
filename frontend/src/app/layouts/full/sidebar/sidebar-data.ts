import { NavItem } from './nav-item/nav-item';

// Role checking functions
export function isSuperAdmin(): boolean {
  const role = localStorage.getItem('role');
  return role === 'SuperAdmin';
}

export function isAgent(): boolean {
  const role = localStorage.getItem('role');
  return role === 'Agent';
}

export function isAdmin(): boolean {
  const role = localStorage.getItem('role');
  return role === 'Admin';
}

export function isMaintenanceAgent(): boolean {
  const role = localStorage.getItem('role');
  const type = localStorage.getItem('type');
  return role === 'Agent' && type === 'maintenance';
}
const Maintenance_NAV_ITEMS: NavItem[] = [
  {
    navCap: 'Equipment',
    roles: ['SuperAdmin', 'Agent']
  },
  {
    displayName: 'Equipment',
    iconName: 'tabler:scuba-diving-tank',
    route: '/Equipment/equipment-list',
    roles: ['SuperAdmin', 'Agent']
  },
  {
    displayName: 'Equipment Stock',
    iconName: 'ic:round-store-mall-directory',
    route: '/EquipmentStock/equipmentStock-list',
    roles: ['SuperAdmin', 'Agent']
  },
  {
    navCap: 'Maintenance',
    roles: ['SuperAdmin', 'Agent']
  },
  {
    displayName: 'Maintenance',
    iconName: 'ix:maintenance',
    route: '/Maintenance/maintenance-list',
    roles: ['SuperAdmin', 'Agent']
  },
  {
    displayName: 'Requests',
    iconName: 'material-symbols:info-rounded',
    route: '/request-maintenance/request-list',
    roles: ['SuperAdmin', 'Agent']
  },

]
// Common navigation items for all roles
const COMMON_NAV_ITEMS: NavItem[] = [
 
  {
    navCap: 'Calendar',
    roles: ['SuperAdmin', 'Admin', 'Agent', 'Technician']
  },
  {
    displayName: 'Calendar',
    iconName: 'fluent:calendar-error-16-regular',
    route: '/Equipment/calendar',
    roles: ['SuperAdmin', 'Admin', 'Agent', 'Technician']
  },
  {
    navCap: 'Messages',
    roles: ['SuperAdmin', 'Admin', 'Agent', 'Technician']
  },
  {
    displayName: 'Messages',
    iconName: 'ant-design:message-outlined',
    route: '/account/app-message-page',
    roles: ['SuperAdmin', 'Admin', 'Agent', 'Technician']
  },
 
];
const AGENT_NAV_ITEMS: NavItem[] = [
  {
    navCap: 'Mission',
    roles: ['SuperAdmin', 'Agent']
  },
  {
    displayName: 'Missions',
    iconName: 'qlementine-icons:task-soon-16',
    route: '/Mission/mission-list',
    roles: ['SuperAdmin',  'Agent']
  },
  {
    navCap: 'Training',
    roles: ['SuperAdmin',  'Agent']
  },
  {
    displayName: 'Training',
    iconName: 'hugeicons:workout-run',
    route: '/training/training-list',
    roles: ['SuperAdmin', 'Agent']
  },
 
];
// Admin-specific navigation items (for SuperAdmin and Admin)
const ADMIN_NAV_ITEMS: NavItem[] = [
  {
    navCap: 'Account',
    roles: ['SuperAdmin', 'Admin']
  },
  {
    displayName: 'Accounts',
    iconName: 'mdi:accounts-outline',
    route: '/account/account-list',
    roles: ['SuperAdmin', 'Admin']
  },
  {
    navCap: 'Mission',
    roles: ['SuperAdmin', 'Admin']
  },
  {
    displayName: 'Missions',
    iconName: 'qlementine-icons:task-soon-16',
    route: '/Mission/mission-list',
    roles: ['SuperAdmin', 'Admin']
  },
  {
    navCap: 'Training',
    roles: ['SuperAdmin', 'Admin']
  },
  {
    displayName: 'Training',
    iconName: 'hugeicons:workout-run',
    route: '/training/training-list',
    roles: ['SuperAdmin', 'Admin']
  },
 
];

// UI Components (SuperAdmin only)
const UI_COMPONENTS_NAV_ITEMS: NavItem[] = [
  {
    navCap: 'UI Components',
    divider: true,
    roles: ['SuperAdmin']
  },
  {
    displayName: 'Badge',
    iconName: 'solar:archive-minimalistic-line-duotone',
    route: '/ui-components/badge',
    roles: ['SuperAdmin']
  },
  {
    displayName: 'Chips',
    iconName: 'solar:danger-circle-line-duotone',
    route: '/ui-components/chips',
    roles: ['SuperAdmin']
  },
  {
    displayName: 'Lists',
    iconName: 'solar:bookmark-square-minimalistic-line-duotone',
    route: '/ui-components/lists',
    roles: ['SuperAdmin']
  },
  {
    displayName: 'Menu',
    iconName: 'solar:file-text-line-duotone',
    route: '/ui-components/menu',
    roles: ['SuperAdmin']
  },
  {
    displayName: 'Tooltips',
    iconName: 'solar:text-field-focus-line-duotone',
    route: '/ui-components/tooltips',
    roles: ['SuperAdmin']
  },
  {
    displayName: 'Forms',
    iconName: 'solar:file-text-line-duotone',
    route: '/ui-components/forms',
    roles: ['SuperAdmin']
  },
  {
    displayName: 'Tables',
    iconName: 'solar:tablet-line-duotone',
    route: '/ui-components/tables',
    roles: ['SuperAdmin']
  }
];



// Extra (SuperAdmin only)
const EXTRA_NAV_ITEMS: NavItem[] = [
  {
    navCap: 'Extra',
    divider: true,
    roles: ['SuperAdmin']
  },
  {
    displayName: 'Icons',
    iconName: 'solar:sticker-smile-circle-2-line-duotone',
    route: '/extra/icons',
    roles: ['SuperAdmin,Agent']
  },
  {
    displayName: 'Sample Page',
    iconName: 'solar:planet-3-line-duotone',
    route: '/extra/sample-page',
    roles: ['SuperAdmin']
  }
];

export function getNavItems(): NavItem[] {
  const role = localStorage.getItem('role');
  const type = localStorage.getItem('type');
  console.log('From localStorage:', localStorage.getItem('role'));

  if (!role) {
    // Return empty or minimal nav if role isn't set yet
    return [];
  }

  console.log('User role:', role);
  // Start with common items filtered by role
  let filteredItems = COMMON_NAV_ITEMS.filter(item => 
    !item.roles || item.roles.includes(role)
  );

  // Add admin items if user is SuperAdmin or Admin
  if (role === 'SuperAdmin' || role === 'Admin') {
    filteredItems = [
      ...filteredItems,
      ...ADMIN_NAV_ITEMS.filter(item => (item.roles ?? []).includes(role)),
     
    ];
  }

  // Add UI components, Auth, and Extra if SuperAdmin
  if (role === 'SuperAdmin' || role === 'Agent' && type==='operation') {
    filteredItems = [
      ...filteredItems,
     
     
    
    ];
  }
    // Add UI components, Auth, and Extra if SuperAdmin
    if (role === 'SuperAdmin' || (role === 'Agent' && type==='maintenance') ) {
      filteredItems = [
        ...filteredItems,
        ...Maintenance_NAV_ITEMS,
        
      ];
    }

  return filteredItems;
}

export function getUserRole(): string | null {
  return localStorage.getItem('role');
}

// Export all nav items for testing/development purposes
export const ALL_NAV_ITEMS: NavItem[] = [
  ...COMMON_NAV_ITEMS,
  ...ADMIN_NAV_ITEMS,
  

 
];