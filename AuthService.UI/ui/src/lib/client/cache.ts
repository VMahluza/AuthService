import type { CacheEntry, CacheStats } from './types';

/**
 * Cache Management Module
 * Single Responsibility: Handle GET request caching
 */

const cache = new Map<string, CacheEntry<unknown>>();
const DEFAULT_CACHE_TTL = 5 * 60 * 1000; // 5 minutes in milliseconds

/**
 * Generate cache key from URL and token
 */
export function generateCacheKey(url: string, token?: string): string {
  return `${url}_${token ? 'authenticated' : 'public'}`;
}

/**
 * Check if cache entry is still valid
 */
export function isCacheValid<T>(entry: CacheEntry<T>): boolean {
  return Date.now() < entry.expiresAt;
}

/**
 * Get data from cache
 */
export function getFromCache<T>(key: string): T | null {
  const entry = cache.get(key) as CacheEntry<T> | undefined;
  
  if (!entry) {
    return null;
  }

  if (isCacheValid(entry)) {
    if (process.env.NODE_ENV === 'development') {
      console.log('💾 Cache Hit:', key);
    }
    return entry.data;
  }

  // Remove expired entry
  cache.delete(key);
  if (process.env.NODE_ENV === 'development') {
    console.log('🗑️ Cache Expired:', key);
  }
  return null;
}

/**
 * Store data in cache
 */
export function setCache<T>(key: string, data: T, ttl: number = DEFAULT_CACHE_TTL): void {
  const entry: CacheEntry<T> = {
    data,
    timestamp: Date.now(),
    expiresAt: Date.now() + ttl,
  };
  cache.set(key, entry);
  
  if (process.env.NODE_ENV === 'development') {
    console.log('💾 Cache Set:', key, `(TTL: ${ttl}ms)`);
  }
}

/**
 * Clear cache for specific key or all cache
 */
export function clearCache(key?: string): void {
  if (key) {
    cache.delete(key);
    if (process.env.NODE_ENV === 'development') {
      console.log('🗑️ Cache Cleared:', key);
    }
  } else {
    cache.clear();
    if (process.env.NODE_ENV === 'development') {
      console.log('🗑️ Cache Cleared: All entries');
    }
  }
}

/**
 * Get cache statistics
 */
export function getCacheStats(): CacheStats {
  const entries = Array.from(cache.entries());
  const valid = entries.filter(([, entry]) => isCacheValid(entry));
  const expired = entries.length - valid.length;

  return {
    total: entries.length,
    valid: valid.length,
    expired,
    size: cache.size,
  };
}
