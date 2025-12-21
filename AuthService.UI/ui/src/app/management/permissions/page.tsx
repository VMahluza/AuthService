'use client';

import { useEffect, useState } from 'react';
import { getPermissionsAction } from './actions';
import styles from '../management.module.css';

interface Permission {
  id: string;
  name: string;
  description: string;
  resource?: string;
  action?: string;
}

export default function PermissionsPage() {
  const [permissions, setPermissions] = useState<Permission[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>('');

  const loadPermissions = async () => {
    const token = localStorage.getItem('accessToken');
    if (!token) {
      setError('No access token found');
      setLoading(false);
      return;
    }

    try {
      const result = await getPermissionsAction(token);
      if (result.success && result.data) {
        setPermissions(result.data);
        setError('');
      } else {
        setError(result.error || 'Failed to load permissions');
      }
      setLoading(false);
    } catch (error) {
      console.error('Error loading permissions:', error);
      setError(error instanceof Error ? error.message : 'Unknown error occurred');
      setLoading(false);
    }
  };

  useEffect(() => {
    loadPermissions();
  }, []);

  return (
    <>
      <h2>Permissions Management</h2>
      
      {error && (
        <div className={styles.errorMessage}>
          <strong>Error:</strong> {error}
        </div>
      )}
      
      <section>
        <h3>System Permissions</h3>
        {loading ? (
          <p>Loading permissions...</p>
        ) : permissions.length > 0 ? (
          <table border={1}>
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Description</th>
                <th>Resource</th>
                <th>Action</th>
              </tr>
            </thead>
            <tbody>
              {permissions.map((permission) => (
                <tr key={permission.id}>
                  <td>{permission.id}</td>
                  <td>{permission.name}</td>
                  <td>{permission.description}</td>
                  <td>{permission.resource || 'N/A'}</td>
                  <td>{permission.action || 'N/A'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <p>No permissions found.</p>
        )}
      </section>
      
      <hr />
      
      <section>
        <h3>Permission Assignment</h3>
        <p>Use the Roles or Groups pages to assign permissions to roles or groups.</p>
        <ul>
          <li><a href="/management/roles">Manage Roles</a></li>
          <li><a href="/management/groups">Manage Groups</a></li>
        </ul>
      </section>
      
      <hr />
      
      <p>
        <a href="/management/dashboard">Back to Dashboard</a>
      </p>
    </>
  );
}
