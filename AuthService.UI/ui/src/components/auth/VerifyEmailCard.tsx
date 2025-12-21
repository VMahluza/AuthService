import * as React from 'react';
import Box from '@mui/material/Box';
import MuiCard from '@mui/material/Card';
import Link from '@mui/material/Link';
import Typography from '@mui/material/Typography';
import { styled } from '@mui/material/styles';
import SitemarkIcon from '../landing/SitemarkIcon';

const Card = styled(MuiCard)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  alignSelf: 'center',
  width: '100%',
  padding: theme.spacing(4),
  gap: theme.spacing(2),
  boxShadow:
    'hsla(220, 30%, 5%, 0.05) 0px 5px 15px 0px, hsla(220, 25%, 10%, 0.05) 0px 15px 35px -5px',
  [theme.breakpoints.up('sm')]: {
    width: '450px',
  },
  ...theme.applyStyles('dark', {
    boxShadow:
      'hsla(220, 30%, 5%, 0.5) 0px 5px 15px 0px, hsla(220, 25%, 10%, 0.08) 0px 15px 35px -5px',
  }),
}));

interface VerifyEmailCardProps {
  message: string;
  isSuccess: boolean;
}

export default function VerifyEmailCard({ message, isSuccess }: VerifyEmailCardProps) {
  return (
    <Card variant="outlined">
      <Box sx={{ display: { xs: 'flex', md: 'none' } }}>
        <SitemarkIcon />
      </Box>
      <Typography
        component="h1"
        variant="h4"
        sx={{ width: '100%', fontSize: 'clamp(2rem, 10vw, 2.15rem)' }}
      >
        Email Verification
      </Typography>
      <Typography variant="body2" color="text.secondary">
        Please wait while we verify your email address...
      </Typography>
      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
        <Typography color={isSuccess ? 'success.main' : 'text.primary'}>
          {message}
        </Typography>
        <Typography sx={{ textAlign: 'center' }}>
          <Link
            href="/auth/login"
            variant="body2"
            sx={{ alignSelf: 'center' }}
          >
            Go to Login
          </Link>
        </Typography>
      </Box>
    </Card>
  );
}
