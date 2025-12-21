'use client';

import { useEffect, useState } from 'react';
import styles from '../management.module.css';

interface JwtPayload {
  sub?: string;
  userId?: string;
  userName?: string;
  name?: string;
  email?: string;
  role?: string | string[];
  roles?: string | string[];
  [key: string]: unknown;
}

export default function ProfilePage() {
  const [profile, setProfile] = useState<JwtPayload | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>('');

  const loadProfile = async () => {
    const token = localStorage.getItem('accessToken');
    if (!token) {
      setError('No access token found');
      setLoading(false);
      return;
    }

    try {
      // Decode JWT to display basic info
      const payload = JSON.parse(atob(token.split('.')[1])) as JwtPayload;
      setProfile(payload);
      setError('');
      setLoading(false);
    } catch (error) {
      console.error('Error loading profile:', error);
      setError(error instanceof Error ? error.message : 'Failed to decode token');
      setLoading(false);
    }
  };

  useEffect(() => {
    loadProfile();
  }, []);

  if (loading) {
    return <div>Loading profile...</div>;
  }

  return (
    <>
      <h2>User Profile</h2>
      
      {error && (
        <div className={styles.errorMessage}>
          <strong>Error:</strong> {error}
        </div>
      )}
      
      <section>
        <h3>Profile Information</h3>
        {profile ? (
          <dl>
            <dt>User ID:</dt>
            <dd>{profile.sub || profile.userId || 'N/A'}</dd>
            
            <dt>Username:</dt>
            <dd>{profile.userName || profile.name || 'N/A'}</dd>
            
            <dt>Email:</dt>
            <dd>{profile.email || 'N/A'}</dd>
            
            <dt>Roles:</dt>
            <dd>{profile.role || profile.roles || 'N/A'}</dd>
          </dl>
        ) : (
          <p>No profile information available</p>
        )}
      </section>
      
      <hr />
      
      <section>
        <h3>Token Details</h3>
        <details>
          <summary>View Full Token Payload</summary>
          <pre>{JSON.stringify(profile, null, 2)}</pre>
        </details>
      </section>
      
      <hr />
      
      <p>
        <a href="/management/dashboard">Back to Dashboard</a>
      </p>
    </>
  );
}
