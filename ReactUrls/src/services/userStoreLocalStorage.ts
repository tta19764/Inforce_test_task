import type { IUserStorage } from '../interfaces/IUserStorage';
import { type AuthUser } from '../types/AuthUser';
import type { JwtToken } from '../types/JwtToken';

const USER_KEY = 'auth_user';

export const userStoreLocalStorage : IUserStorage = {
    getUser(): AuthUser | null {
        const data = localStorage.getItem(USER_KEY);
        return data ? JSON.parse(data) as AuthUser : null;
    },

    setUser(user: AuthUser): void {
        localStorage.setItem(USER_KEY, JSON.stringify(user));
    },

    clearUser(): void {
        localStorage.removeItem(USER_KEY);
    },

    getToken: function (): JwtToken | null {
        const user = this.getUser();
        return user && user.token;
    },

    setToken: function (token: JwtToken): void {
        const user = this.getUser();

        if(user){
            user.token = token;
            this.setUser(user);
        }
    },

    clearToken: function (): void {
        const user = this.getUser();

        if(user){
            user.token = null;
            this.setUser(user);
        }
    }
};