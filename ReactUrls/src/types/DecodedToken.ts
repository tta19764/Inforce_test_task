export type DecodedToken = {
    name: string;
    nameId: string;
    role: string;
    exp: number;
    iss: string;
    aud: string;
}