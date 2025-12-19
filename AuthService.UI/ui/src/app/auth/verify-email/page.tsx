'use client';

import { useSearchParams } from 'next/navigation';
import { useEffect, useState } from 'react';

export default function VerifyEmailPage() {
  const searchParams = useSearchParams();
  const token = searchParams.get('token');
  const [response, setResponse] = useState('Verifying...');

  useEffect(() => {
    if (token) {
      verifyEmail(token);
    } else {
      setResponse('No verification token provided.');
    }
  }, [token]);

  const verifyEmail = async (token: string) => {
    try {
      const response = await fetch(
        `http://localhost:5102/api/auth/verify-email?token=${encodeURIComponent(token)}`,
        {
          method: 'GET',
        }
      );

      const result = await response.json();
      setResponse(JSON.stringify(result, null, 2));

      if (response.ok) {
        alert('Email verified successfully! You can now login.');
        setTimeout(() => {
          window.location.href = '/auth/login';
        }, 2000);
      }
    } catch (error: any) {
      setResponse('Error: ' + error.message);
    }
  };

  return (
    <>
      <h2>Email Verification</h2>
      <p>Please wait while we verify your email address...</p>
      
      <hr />
      <h3>Verification Status</h3>
      <pre>{response}</pre>

      <p>
        <a href="/auth/login">Go to Login</a>
      </p>
    </>
  );
}
