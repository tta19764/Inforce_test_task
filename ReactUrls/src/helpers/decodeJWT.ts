import type { DecodedToken } from "../types/DecodedToken";

export default function decodeJWT(token: string): DecodedToken | null {
    try {
        const parts = token.split('.');
        
        if (parts.length !== 3) {
            throw new Error('Invalid JWT token');
        }
        
        // Decode the payload (second part)
        const payload = parts[1];
        const decoded = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));

        const parsed = JSON.parse(decoded);
        
        // Map the full URIs to clean property names
        return {
            name: parsed['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
            nameId: parsed['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
            role: parsed['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],
            exp: parsed.exp,
            iss: parsed.iss,
            aud: parsed.aud
        };
    } catch (error) {
        console.error('Error decoding JWT:', error);
        return null;
    }
}