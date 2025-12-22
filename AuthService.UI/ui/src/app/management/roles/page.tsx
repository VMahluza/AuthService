'use client';

import { useEffect, useState } from 'react';
import { getRolesAction, createRoleAction } from './actions';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Grid from '@mui/material/Grid';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import Alert from '@mui/material/Alert';
import CircularProgress from '@mui/material/CircularProgress';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';

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
      setError('No access token found');
      return;
    }

    const name = formData.get('name') as string;
    const description = formData.get('description') as string;

    if (!name) {
      setError('Role name is required');
      return;
    }

    try {
      const result = await createRoleAction(token, name, description);
      if (result.success) {
        loadRoles();
        e.currentTarget.reset();
      } else {
        setError(result.error || 'Failed to create role');
      }
    } catch (error) {
      setError(error instanceof Error ? error.message : 'Unknown error occurred');
    }
  };

  return (
    <Box sx={{ width: '100%', maxWidth: { sm: '100%', md: '1700px' } }}>
      <Typography component="h2" variant="h6" sx={{ mb: 2 }}>
        Roles Management
      </Typography>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <Grid container spacing={2}>
        <Grid size={{ xs: 12, md: 4 }}>
          <Card>
            <CardContent>
              <Typography component="h3" variant="h6" gutterBottom>
                Create New Role
              </Typography>
              <Box component="form" onSubmit={handleCreateRole} sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                <TextField
                  label="Role Name"
                  name="name"
                  required
                  fullWidth
                  variant="outlined"
                />
                <TextField
                  label="Description"
                  name="description"
                  multiline
                  rows={3}
                  fullWidth
                  variant="outlined"
                />
                <Button type="submit" variant="contained" color="primary">
                  Create Role
                </Button>
              </Box>
            </CardContent>
          </Card>
        </Grid>

        <Grid size={{ xs: 12, md: 8 }}>
          <Card>
            <CardContent>
              <Typography component="h3" variant="h6" gutterBottom>
                Existing Roles
              </Typography>
              {loading ? (
                <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
                  <CircularProgress />
                </Box>
              ) : roles.length > 0 ? (
                <TableContainer component={Paper} elevation={0} variant="outlined">
                  <Table aria-label="roles table">
                    <TableHead>
                      <TableRow>
                        <TableCell>ID</TableCell>
                        <TableCell>Name</TableCell>
                        <TableCell>Description</TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {roles.map((role) => (
                        <TableRow key={role.id}>
                          <TableCell>{role.id}</TableCell>
                          <TableCell>{role.name}</TableCell>
                          <TableCell>{role.description}</TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </TableContainer>
              ) : (
                <Typography variant="body1" color="text.secondary">
                  No roles found. Create your first role.
                </Typography>
              )}
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
}
