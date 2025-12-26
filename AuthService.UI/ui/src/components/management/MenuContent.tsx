'use client';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import Stack from '@mui/material/Stack';
import HomeRoundedIcon from '@mui/icons-material/HomeRounded';
import PeopleRoundedIcon from '@mui/icons-material/PeopleRounded';
import AssignmentRoundedIcon from '@mui/icons-material/AssignmentRounded';
import SettingsRoundedIcon from '@mui/icons-material/SettingsRounded';
import SecurityRoundedIcon from '@mui/icons-material/SecurityRounded';
import GroupRoundedIcon from '@mui/icons-material/GroupRounded';
import Link from 'next/link';
import { usePathname } from 'next/navigation';

const mainListItems = [
  { text: 'Dashboard', icon: <HomeRoundedIcon />, href: '/management/dashboard' },
  { text: 'Groups', icon: <GroupRoundedIcon />, href: '/management/groups' },
  { text: 'Roles', icon: <AssignmentRoundedIcon />, href: '/management/roles' },
  { text: 'Permissions', icon: <SecurityRoundedIcon />, href: '/management/permissions' },
  { text: 'Profile', icon: <PeopleRoundedIcon />, href: '/management/profile' },
];

const secondaryListItems = [
  { text: 'Settings', icon: <SettingsRoundedIcon />, href: '/management/settings' },
];

export default function MenuContent() {
  const pathname = usePathname();

  return (
    <Stack sx={{ flexGrow: 1, p: 1, justifyContent: 'space-between' }}>
      <List dense>
        {mainListItems.map((item, index) => (
          <ListItem key={index} disablePadding sx={{ display: 'block' }}>
            <ListItemButton 
              component={Link} 
              href={item.href}
              selected={pathname === item.href}
            >
              <ListItemIcon>{item.icon}</ListItemIcon>
              <ListItemText primary={item.text} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
      <List dense>
        {secondaryListItems.map((item, index) => (
          <ListItem key={index} disablePadding sx={{ display: 'block' }}>
            <ListItemButton
              component={Link}
              href={item.href}
              selected={pathname === item.href}
            >
              <ListItemIcon>{item.icon}</ListItemIcon>
              <ListItemText primary={item.text} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </Stack>
  );
}
