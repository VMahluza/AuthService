'use client';

import { useEffect, useState } from 'react';
import { getGroupsAction, createGroupAction } from './actions';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Grid from '@mui/material/Grid2';
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
      setError('No access token found');
      return;
    }

    const name = formData.get('name') as string;
    const description = formData.get('description') as string;

    if (!name) {
      setError('Group name is required');
      return;
    }

    try {
      const result = await createGroupAction(token, name, description);
      if (result.success) {
        loadGroups();
        e.currentTarget.reset();
      } else {
        setError(result.error || 'Failed to create group');
      }
    } catch (error) {
      setError(error instanceof Error ? error.message : 'Unknown error occurred');
    }
  };

  return (
    <Box sx={{ width: '100%', maxWidth: { sm: '100%', md: '1700px' } }}>
      <Typography component="h2" variant="h6" sx={{ mb: 2 }}>
        Groups Management
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
                Create New Group
              </Typography>
              <Box component="form" onSubmit={handleCreateGroup} sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                <TextField
                  label="Group Name"
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
                  Create Group
                </Button>
              </Box>
            </CardContent>
          </Card>
        </Grid>

        <Grid size={{ xs: 12, md: 8 }}>
          <Card>
            <CardContent>
              <Typography component="h3" variant="h6" gutterBottom>
                Existing Groups
              </Typography>
              {loading ? (
                <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
                  <CircularProgress />
                </Box>
              ) : groups.length > 0 ? (
                <TableContainer component={Paper} elevation={0} variant="outlined">
                  <Table aria-label="groups table">
                    <TableHead>
                      <TableRow>
                        <TableCell>ID</TableCell>
                        <TableCell>Name</TableCell>
                        <TableCell>Description</TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {groups.map((group) => (
                        <TableRow key={group.id}>
                          <TableCell>{group.id}</TableCell>
                          <TableCell>{group.name}</TableCell>
                          <TableCell>{group.description}</TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </TableContainer>
              ) : (
                <Typography variant="body1" color="text.secondary">
                  No groups found. Create your first group.
                </Typography>
              )}
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
}
