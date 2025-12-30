import type { IUserStorage } from '../interfaces/IUserStorage';
import { type AuthUser } from '../types/AuthUser';
import type { JwtToken } from '../types/JwtToken';
import type { StorageUser } from '../types/StorageUser';

const USER_KEY = 'auth_user';

export const userStoreLocalStorage : IUserStorage = {
    getUser(): AuthUser | null {
        const data = localStorage.getItem(USER_KEY);
        return data ? JSON.parse(data) as StorageUser : null;
    },

    setUser(user: AuthUser, token: JwtToken | null): void {
        const storageUser: StorageUser = { ...user, token };
        localStorage.setItem(USER_KEY, JSON.stringify(storageUser));
    },

    clearUser(): void {
        localStorage.removeItem(USER_KEY);
    },

    getToken: function (): JwtToken | null {
        const data = localStorage.getItem(USER_KEY);
        if(data){
            const storageUser = JSON.parse(data) as StorageUser;
            return storageUser.token;
        }

        return null;
    },

    setToken: function (token: JwtToken): void {
        const data = localStorage.getItem(USER_KEY);
        if(data){
            const storageUser = JSON.parse(data) as StorageUser;
            storageUser.token = token;
            this.setUser(storageUser, token);
        }
    },

    clearToken: function (): void {
        const data = localStorage.getItem(USER_KEY);
        if(data){
            const storageUser = JSON.parse(data) as StorageUser;
            storageUser.token = null;
            this.setUser(storageUser, null);
        }
    }
};