'use server';

import axios, { AxiosError } from 'axios';

interface Group {
  id: string;
  name: string;
  description: string;
}

interface ApiResponse<T> {
  success: boolean;
  data?: T;
  error?: string;
}

export async function getGroupsAction(token: string): Promise<ApiResponse<Group[]>> {
  try {
    const response = await axios.get<Group[]>('http://localhost:5102/api/groups', {
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
        error: error.response?.data?.message || error.message || 'Failed to fetch groups'
      };
    }
    return { 
      success: false, 
      error: error instanceof Error ? error.message : 'Unknown error occurred' 
    };
  }
}

export async function createGroupAction(
  token: string,
  name: string,
  description: string
): Promise<ApiResponse<Group>> {
  try {
    const response = await axios.post<Group>(
      'http://localhost:5102/api/groups',
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
        error: error.response?.data?.message || error.message || 'Failed to create group'
      };
    }
    return { 
      success: false, 
      error: error instanceof Error ? error.message : 'Unknown error occurred' 
    };
  }
}
