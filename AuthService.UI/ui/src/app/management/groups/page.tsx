'use client';

import { useEffect, useState } from 'react';
import { getGroupsAction, createGroupAction } from './actions';
import styles from '../management.module.css';

interface Group {
  id: string;
  name: string;
  description: string;
}

export default function GroupsPage() {
  const [groups, setGroups] = useState<Group[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>('');

  const loadGroups = async () => {
    const token = localStorage.getItem('accessToken');
    if (!token) {
      setError('No access token found');
      setLoading(false);
      return;
    }

    try {
      const result = await getGroupsAction(token);
      if (result.success && result.data) {
        setGroups(result.data);
        setError('');
      } else {
        setError(result.error || 'Failed to load groups');
      }
      setLoading(false);
    } catch (error) {
      console.error('Error loading groups:', error);
      setError(error instanceof Error ? error.message : 'Unknown error occurred');
      setLoading(false);
    }
  };

  useEffect(() => {
    loadGroups();
  }, []);

  const handleCreateGroup = async (e: React.FormEvent<HTMLFormElement>) => {
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
      alert('Group name is required');
      return;
    }

    try {
      const result = await createGroupAction(token, name, description);
      if (result.success) {
        alert('Group created successfully!');
        loadGroups();
        e.currentTarget.reset();
      } else {
        alert('Error: ' + (result.error || 'Failed to create group'));
      }
    } catch (error) {
      alert('Error: ' + (error instanceof Error ? error.message : 'Unknown error occurred'));
    }
  };

  return (
    <>
      <h2>Groups Management</h2>
      
      {error && (
        <div className={styles.errorMessage}>
          <strong>Error:</strong> {error}
        </div>
      )}
      
      <section>
        <h3>Create New Group</h3>
        <form onSubmit={handleCreateGroup}>
          <fieldset>
            <legend>Group Information</legend>
            
            <label htmlFor="name">Group Name:</label>
            <input type="text" id="name" name="name" required />
            <br /><br />
            
            <label htmlFor="description">Description:</label>
            <textarea id="description" name="description" rows={3}></textarea>
            <br /><br />
            
            <button type="submit">Create Group</button>
          </fieldset>
        </form>
      </section>
      
      <hr />
      
      <section>
        <h3>Existing Groups</h3>
        {loading ? (
          <p>Loading groups...</p>
        ) : groups.length > 0 ? (
          <table border={1}>
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Description</th>
              </tr>
            </thead>
            <tbody>
              {groups.map((group) => (
                <tr key={group.id}>
                  <td>{group.id}</td>
                  <td>{group.name}</td>
                  <td>{group.description}</td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <p>No groups found. Create your first group above.</p>
        )}
      </section>
      
      <hr />
      
      <p>
        <a href="/management/dashboard">Back to Dashboard</a>
      </p>
    </>
  );
}
