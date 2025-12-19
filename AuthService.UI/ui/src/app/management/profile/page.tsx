'use client';

import { useEffect, useState } from 'react';

export default function ProfilePage() {
  const [profile, setProfile] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadProfile();
  }, []);

  const loadProfile = async () => {
    const token = localStorage.getItem('accessToken');
    if (!token) return;

    try {
      // Decode JWT to display basic info
      const payload = JSON.parse(atob(token.split('.')[1]));
      setProfile(payload);
      setLoading(false);
    } catch (error) {
      console.error('Error loading profile:', error);
      setLoading(false);
    }
  };

  if (loading) {
    return <div>Loading profile...</div>;
  }

  return (
    <>
      <h2>User Profile</h2>
      
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
