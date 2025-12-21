import * as React from 'react';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import MuiCard from '@mui/material/Card';
import Checkbox from '@mui/material/Checkbox';
import FormControlLabel from '@mui/material/FormControlLabel';
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

interface LogoutCardProps {
  action: (payload: FormData) => void;
}

export default function LogoutCard({ action }: LogoutCardProps) {
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
        Logout
      </Typography>
      <Typography variant="body2" color="text.secondary">
        Sign out from your account.
      </Typography>
      <Box
        component="form"
        action={action}
        sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}
      >
        <FormControlLabel
          control={<Checkbox name="revokeAllSessions" color="primary" />}
          label="Revoke all sessions"
        />
        <Button
          type="submit"
          fullWidth
          variant="contained"
          color="error"
        >
          Logout
        </Button>
        <Typography sx={{ textAlign: 'center' }}>
          <Link
            href="/management/dashboard"
            variant="body2"
            sx={{ alignSelf: 'center' }}
          >
            Back to Dashboard
          </Link>
        </Typography>
      </Box>
    </Card>
  );
}
