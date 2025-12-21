'use server';

import axios, { AxiosError } from 'axios';

interface Role {
  id: string;
  name: string;
  description: string;
}

interface ApiResponse<T> {
  success: boolean;
  data?: T;
  error?: string;
}

export async function getRolesAction(token: string): Promise<ApiResponse<Role[]>> {
  try {
    const response = await axios.get<Role[]>('http://localhost:5102/api/roles', {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    });

    return { success: true, data: response.data };
  } catch (error) {
    if (error instanceof AxiosError) {
      return { 
        success: false, 
        error: error.response?.data?.message || error.message || 'Failed to fetch roles'
      };
    }
    return { 
      success: false, 
      error: error instanceof Error ? error.message : 'Unknown error occurred' 
    };
  }
}

export async function createRoleAction(
  token: string,
  name: string,
  description: string
): Promise<ApiResponse<Role>> {
  try {
    const response = await axios.post<Role>(
      'http://localhost:5102/api/roles',
      { name, description },
      {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      }
    );

    return { success: true, data: response.data };
  } catch (error) {
    if (error instanceof AxiosError) {
      return { 
        success: false, 
        error: error.response?.data?.message || error.message || 'Failed to create role'
      };
    }
    return { 
      success: false, 
      error: error instanceof Error ? error.message : 'Unknown error occurred' 
    };
  }
}
