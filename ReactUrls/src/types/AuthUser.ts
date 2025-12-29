import type { JwtToken } from "./JwtToken";

export type AuthUser = {
    userId: string;
    nickname: string;
    role: string;
    token: JwtToken | null;
}