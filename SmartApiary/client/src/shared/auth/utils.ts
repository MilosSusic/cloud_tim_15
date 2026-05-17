/**
 * Auth utility funkcije za JWT role parsing
 */

export function getUserRoleFromToken(token: string | null): string | null {
  if (!token) return null;

  try {
    const payload = JSON.parse(atob(token.split('.')[1])) as Record<string, string>;

    return (
      payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
      payload['role'] ||
      null
    );
  } catch {
    return null;
  }
}
