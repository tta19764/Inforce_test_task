import { type AuthUser } from "../types/AuthUser";
import type { JwtToken } from "../types/JwtToken";

export interface IUserStorage {
    getToken(): JwtToken | null;
    setToken(token: JwtToken): void;
    clearToken(): void;

    getUser(): AuthUser | null;
    setUser(user: AuthUser, token: JwtToken | null): void;
    clearUser(): void;
}