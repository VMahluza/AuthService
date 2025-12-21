'use client';

import { useEffect, useState } from 'react';
import { getRolesAction, createRoleAction } from './actions';
import styles from '../management.module.css';

interface Role {
  id: string;
  name: string;
  description: string;
}

export default function RolesPage() {
  const [roles, setRoles] = useState<Role[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>('');

  const loadRoles = async () => {
    const token = localStorage.getItem('accessToken');
    if (!token) {
      setError('No access token found');
      setLoading(false);
      return;
    }

    try {
      const result = await getRolesAction(token);
      if (result.success && result.data) {
        setRoles(result.data);
        setError('');
      } else {
        setError(result.error || 'Failed to load roles');
      }
      setLoading(false);
    } catch (error) {
      console.error('Error loading roles:', error);
      setError(error instanceof Error ? error.message : 'Unknown error occurred');
      setLoading(false);
    }
  };

  useEffect(() => {
    loadRoles();
  }, []);

  const handleCreateRole = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);
    const token = localStorage.getItem('accessToken');

    if (!token) {
      alert('No access token found');
      return;
    }

    const name = formData.get('name') as string;
    const description = formData.get('description') as string;

    if (!name) {
      alert('Role name is required');
      return;
    }

    try {
      const result = await createRoleAction(token, name, description);
      if (result.success) {
        alert('Role created successfully!');
        loadRoles();
        e.currentTarget.reset();
      } else {
        alert('Error: ' + (result.error || 'Failed to create role'));
      }
    } catch (error) {
      alert('Error: ' + (error instanceof Error ? error.message : 'Unknown error occurred'));
    }
  };

  return (
    <>
      <h2>Roles Management</h2>
      
      {error && (
        <div className={styles.errorMessage}>
          <strong>Error:</strong> {error}
        </div>
      )}
      
      <section>
        <h3>Create New Role</h3>
        <form onSubmit={handleCreateRole}>
          <fieldset>
            <legend>Role Information</legend>
            
            <label htmlFor="name">Role Name:</label>
            <input type="text" id="name" name="name" required />
            <br /><br />
            
            <label htmlFor="description">Description:</label>
            <textarea id="description" name="description" rows={3}></textarea>
            <br /><br />
            
            <button type="submit">Create Role</button>
          </fieldset>
        </form>
      </section>
      
      <hr />
      
      <section>
        <h3>Existing Roles</h3>
        {loading ? (
          <p>Loading roles...</p>
        ) : roles.length > 0 ? (
          <table border={1}>
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Description</th>
              </tr>
            </thead>
            <tbody>
              {roles.map((role) => (
                <tr key={role.id}>
                  <td>{role.id}</td>
                  <td>{role.name}</td>
                  <td>{role.description}</td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <p>No roles found. Create your first role above.</p>
        )}
      </section>
      
      <hr />
      
      <p>
        <a href="/management/dashboard">Back to Dashboard</a>
      </p>
    </>
  );
}
