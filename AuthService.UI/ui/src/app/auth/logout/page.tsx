'use client';

import { logoutAction } from './actions';
import { useEffect } from 'react';
import { clearTokens } from '@/lib/token-utils';
import LogoutCard from '@/components/auth/LogoutCard';

export default function LogoutPage() {
  // Clear client-side tokens on mount
  useEffect(() => {
    clearTokens();
  }, []);

  return (
    <LogoutCard action={logoutAction} />
  );
}
