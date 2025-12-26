import * as React from 'react';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import MuiCard from '@mui/material/Card';
import FormLabel from '@mui/material/FormLabel';
import FormControl from '@mui/material/FormControl';
import Link from '@mui/material/Link';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import { styled } from '@mui/material/styles';
import SitemarkIcon from '../landing/SitemarkIcon';
import NextLink from 'next/link';

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

interface ResetPasswordCardProps {
  action: (payload: FormData) => void;
  isPending?: boolean;
  error?: string;
  success?: boolean;
  message?: string;
  token: string;
}

export default function ResetPasswordCard({ action, isPending, error, success, message, token }: ResetPasswordCardProps) {
  const [passwordError, setPasswordError] = React.useState(false);
  const [passwordErrorMessage, setPasswordErrorMessage] = React.useState('');

  const validateInputs = () => {
    const password = document.getElementById('newPassword') as HTMLInputElement;

    let isValid = true;

    if (!password.value || password.value.length < 6) {
      setPasswordError(true);
      setPasswordErrorMessage('Password must be at least 6 characters long.');
      isValid = false;
    } else {
      setPasswordError(false);
      setPasswordErrorMessage('');
    }

    return isValid;
  };

  return (
    <Card variant="outlined">
      <Box sx={{ display: { xs: 'flex', md: 'none' } }}>
        <NextLink href="/">
          <SitemarkIcon />
        </NextLink>
      </Box>
      <Typography
        component="h1"
        variant="h4"
        sx={{ width: '100%', fontSize: 'clamp(2rem, 10vw, 2.15rem)' }}
      >
        Reset Password
      </Typography>
      <Typography variant="body2" color="text.secondary">
        Enter your new password below.
      </Typography>
      <Box
        component="form"
        action={action}
        onSubmit={(event) => {
          if (!validateInputs()) {
            event.preventDefault();
          }
        }}
        sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}
      >
        <FormControl>
          <FormLabel htmlFor="token">Reset Token</FormLabel>
          <TextField
            required
            fullWidth
            id="token"
            name="token"
            defaultValue={token}
            variant="outlined"
          />
        </FormControl>
        <FormControl>
          <FormLabel htmlFor="newPassword">New Password</FormLabel>
          <TextField
            required
            fullWidth
            name="newPassword"
            placeholder=""
            type="password"
            id="newPassword"
            autoComplete="new-password"
            variant="outlined"
            error={passwordError}
            helperText={passwordErrorMessage}
            color={passwordError ? 'error' : 'primary'}
          />
        </FormControl>
        {error && <Typography color="error">{error}</Typography>}
        {success && <Typography color="success.main">{message}</Typography>}
        <Button
          type="submit"
          fullWidth
          variant="contained"
          disabled={isPending}
        >
          {isPending ? 'Resetting...' : 'Reset Password'}
        </Button>
        <Typography sx={{ textAlign: 'center' }}>
          <Link
            href="/auth/login"
            variant="body2"
            sx={{ alignSelf: 'center' }}
          >
            Back to Login
          </Link>
        </Typography>
      </Box>
    </Card>
  );
}
