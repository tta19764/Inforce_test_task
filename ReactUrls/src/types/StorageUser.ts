import type { JwtToken } from "./JwtToken";

export type StorageUser = {
    userId: string;
    nickname: string;
    role: string;
    token: JwtToken | null;
}