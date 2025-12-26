import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import SecurityRoundedIcon from '@mui/icons-material/SecurityRounded';
import SpeedRoundedIcon from '@mui/icons-material/SpeedRounded';
import VerifiedUserRoundedIcon from '@mui/icons-material/VerifiedUserRounded';
import LockRoundedIcon from '@mui/icons-material/LockRounded';
import SitemarkIcon from '../landing/SitemarkIcon';

const items = [
  {
    icon: <SecurityRoundedIcon sx={{ color: 'text.secondary' }} />,
    title: 'Enterprise-grade Security',
    description:
      'Protect your data with industry-standard encryption and security protocols.',
  },
  {
    icon: <SpeedRoundedIcon sx={{ color: 'text.secondary' }} />,
    title: 'High Performance',
    description:
      'Optimized for speed and reliability to ensure seamless user experience.',
  },
  {
    icon: <VerifiedUserRoundedIcon sx={{ color: 'text.secondary' }} />,
    title: 'Identity Management',
    description:
      'Comprehensive identity and access management for your applications.',
  },
  {
    icon: <LockRoundedIcon sx={{ color: 'text.secondary' }} />,
    title: 'Secure Authentication',
    description:
      'Robust authentication mechanisms including JWT, OAuth2, and OpenID Connect.',
  },
];

export default function Content() {
  return (
    <Stack
      sx={{ flexDirection: 'column', alignSelf: 'center', gap: 4, maxWidth: 450 }}
    >
      <Box sx={{ display: { xs: 'none', md: 'flex' } }}>
        <SitemarkIcon />
      </Box>
      {items.map((item, index) => (
        <Stack key={index} direction="row" sx={{ gap: 2 }}>
          {item.icon}
          <div>
            <Typography gutterBottom sx={{ fontWeight: 'medium' }}>
              {item.title}
            </Typography>
            <Typography variant="body2" sx={{ color: 'text.secondary' }}>
              {item.description}
            </Typography>
          </div>
        </Stack>
      ))}
    </Stack>
  );
}
