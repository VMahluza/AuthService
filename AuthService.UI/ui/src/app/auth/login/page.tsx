'use client';

import { useActionState, useEffect } from 'react';
import { loginAction, LoginState } from '@/app/auth/login/actions';
import { useRouter } from 'next/navigation';
import SignInCard from '@/components/auth/SignInCard';

const initialState: LoginState = {
  message: '',
  error: '',
  success: false,
};

export default function LoginPage() {
  const [state, formAction, isPending] = useActionState(loginAction, initialState);
  const router = useRouter();

  // Sync tokens to localStorage on successful login
  useEffect(() => {
    if (state.success && state.tokens) {
      localStorage.setItem('accessToken', state.tokens.accessToken);
      localStorage.setItem('refreshToken', state.tokens.refreshToken);
      // Redirect after storing tokens
      router.push('/management/dashboard');
    }
  }, [state.success, state.tokens, router]);

  return (
    <SignInCard action={formAction} isPending={isPending} error={state.error} />
  );
}
