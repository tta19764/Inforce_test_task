import type { AuthUser } from "./AuthUser";

export type UserState = {
    isLoggedIn: boolean;
    userData: AuthUser | null;
}