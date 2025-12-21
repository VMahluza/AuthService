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

interface RefreshTokenCardProps {
  action: (payload: FormData) => void;
  isPending?: boolean;
  error?: string;
  success?: boolean;
  message?: string;
  refreshToken: string;
  onRefreshTokenChange: (value: string) => void;
}

export default function RefreshTokenCard({ action, isPending, error, success, message, refreshToken, onRefreshTokenChange }: RefreshTokenCardProps) {
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
        Refresh Token
      </Typography>
      <Typography variant="body2" color="text.secondary">
        Use your refresh token to get a new access token.
      </Typography>
      <Box
        component="form"
        action={action}
        sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}
      >
        <FormControl>
          <FormLabel htmlFor="refreshToken">Refresh Token</FormLabel>
          <TextField
            required
            fullWidth
            id="refreshToken"
            name="refreshToken"
            value={refreshToken}
            onChange={(e) => onRefreshTokenChange(e.target.value)}
            variant="outlined"
            multiline
            rows={4}
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
          {isPending ? 'Refreshing...' : 'Refresh Token'}
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
