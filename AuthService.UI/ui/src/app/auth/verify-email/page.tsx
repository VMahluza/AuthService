'use client';

import { useSearchParams, useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import { verifyEmailAction } from './action';

export default function VerifyEmailPage() {
  const searchParams = useSearchParams();
  const router = useRouter();
  const token = searchParams.get('token');
  const [response, setResponse] = useState(token ? 'Verifying...' : 'No verification token provided.');
  const [isSuccess, setIsSuccess] = useState(false);

  useEffect(() => {
    if (!token) {
      return;
    }

    const verifyEmail = async () => {
      try {
        const result = await verifyEmailAction(token);
        
        if (result.success) {
          setIsSuccess(true);
          setResponse(result.message || 'Email verified successfully!');
          setTimeout(() => {
            router.push('/auth/login');
          }, 2000);
        } else {
          setResponse(result.error || 'Failed to verify email');
        }
      } catch (error: unknown) {
        setResponse('Error: ' + (error instanceof Error ? error.message : 'Unknown error'));
      }
    };

    verifyEmail();
  }, [token, router]);

  return (
    <>
      <h2>Email Verification</h2>
      <p>Please wait while we verify your email address...</p>
      
      <hr />
      <h3>Verification Status</h3>
      <pre style={{ color: isSuccess ? 'green' : 'inherit' }}>{response}</pre>

      <p>
        <a href="/auth/login">Go to Login</a>
      </p>
    </>
  );
}
