'use client';

import { useState } from 'react';

interface CreateRoleCardProps {
  onRoleCreated?: () => void;
}

export default function CreateRoleCard({ onRoleCreated }: CreateRoleCardProps) {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setSuccess(false);

    try {
      const response = await fetch('/api/roles', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ name, description }),
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Failed to create role');
      }

      setSuccess(true);
      setName('');
      setDescription('');
      
      if (onRoleCreated) {
        onRoleCreated();
      }

      setTimeout(() => setSuccess(false), 3000);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{
      border: '1px solid #e0e0e0',
      borderRadius: '8px',
      padding: '16px',
      backgroundColor: '#fff',
      boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
    }}>
      <h3 style={{ marginTop: 0, marginBottom: '16px', fontSize: '1.25rem', fontWeight: 600 }}>
        Create New Role
      </h3>
      
      {error && (
        <div style={{
          padding: '12px 16px',
          marginBottom: '16px',
          backgroundColor: '#fee',
          border: '1px solid #fcc',
          borderRadius: '4px',
          color: '#c00'
        }}>
          {error}
        </div>
      )}
      
      {success && (
        <div style={{
          padding: '12px 16px',
          marginBottom: '16px',
          backgroundColor: '#efe',
          border: '1px solid #cfc',
          borderRadius: '4px',
          color: '#080'
        }}>
          Role created successfully!
        </div>
      )}

      <form onSubmit={handleSubmit} noValidate>
        <div style={{ marginBottom: '16px' }}>
          <label htmlFor="role-name" style={{ display: 'block', marginBottom: '8px', fontWeight: 500 }}>
            Role Name *
          </label>
          <input
            id="role-name"
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            required
            disabled={loading}
            style={{
              width: '100%',
              padding: '10px 12px',
              fontSize: '1rem',
              border: '1px solid #ccc',
              borderRadius: '4px',
              boxSizing: 'border-box',
              outline: 'none'
            }}
          />
        </div>
        
        <div style={{ marginBottom: '16px' }}>
          <label htmlFor="role-description" style={{ display: 'block', marginBottom: '8px', fontWeight: 500 }}>
            Description
          </label>
          <textarea
            id="role-description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            rows={3}
            disabled={loading}
            style={{
              width: '100%',
              padding: '10px 12px',
              fontSize: '1rem',
              border: '1px solid #ccc',
              borderRadius: '4px',
              boxSizing: 'border-box',
              resize: 'vertical',
              fontFamily: 'inherit',
              outline: 'none'
            }}
          />
        </div>
        
        <button
          type="submit"
          disabled={loading || !name.trim()}
          style={{
            width: '100%',
            padding: '12px 24px',
            fontSize: '1rem',
            fontWeight: 500,
            color: '#fff',
            backgroundColor: (loading || !name.trim()) ? '#ccc' : '#1976d2',
            border: 'none',
            borderRadius: '4px',
            cursor: (loading || !name.trim()) ? 'not-allowed' : 'pointer',
            marginTop: '16px'
          }}
        >
          {loading ? 'Creating...' : 'Create Role'}
        </button>
      </form>
    </div>
  );
}