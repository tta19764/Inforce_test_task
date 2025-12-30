import { 
    createSlice, 
    createAsyncThunk 
} from "@reduxjs/toolkit";
import { type UserState } from "../../types/UserState";
import axiosPublic from "../../api/axios";
import type { AuthUser } from "../../types/AuthUser";
import type { LoginRequest } from "../../types/LoginRequest";
import decodeJWT from "../../helpers/decodeJWT";
import type { DecodedToken } from "../../types/DecodedToken";
import type { JwtToken } from "../../types/JwtToken";
import { userStoreLocalStorage } from "../../services/userStoreLocalStorage";
import type { IUserStorage } from "../../interfaces/IUserStorage";

const AUTH_URL = import.meta.env.VITE_APP_AUTH_ENDPOINT;

const userStorage : IUserStorage = userStoreLocalStorage;

const storedUser = userStorage.getUser();

const initialState: UserState = {
    isLoggedIn: storedUser !== null,
    userData: storedUser,
}

export const loginUser = createAsyncThunk(
    "user/login",
    async (payload: LoginRequest, thunkAPI) => {
        try {
            const response = await axiosPublic.post(AUTH_URL+"login", JSON.stringify(payload),
        {
        headers: {
            "Content-Type": "application/json",
        },
    });
            return response.data;
        } catch (error) { 
            console.log(error); return thunkAPI.rejectWithValue("Failed to login"); 
        }
    });

const userSlice = createSlice({
    name: "user",
    initialState,
    reducers: {
        logout: (state) => {
            userStorage.clearUser();
            state.isLoggedIn = false;
            state.userData = null;
        },
        updateToken: (state, action) => {
            if(state.userData){
                state.userData.token = action.payload;
                userStorage.setToken(action.payload);
            };
        },
    },
    extraReducers: (builder) => {
        builder
            .addCase(loginUser.pending, (state) => {
                state.userData = null;
                state.isLoggedIn = false;
            })
            .addCase(loginUser.fulfilled, (state, action) => {
                const receivedToken : JwtToken = action.payload;
                const decodedJWT : DecodedToken | null = decodeJWT(receivedToken.accessToken); 
                if(decodedJWT !== null){ 
                    const user : AuthUser = {
                        userId: decodedJWT.nameId, 
                        role: decodedJWT.role, 
                        nickname: decodedJWT.name, 
                        token: receivedToken,
                    };

                    userStorage.setUser(user);
                    state.userData = user;
                    state.isLoggedIn = true;
                }
                else{
                    state.userData = null;
                    state.isLoggedIn = false;
                }
            })
            .addCase(loginUser.rejected, (state) => {
                state.userData = null;
                state.isLoggedIn = false;
            });
    }
});

export const { logout, updateToken } = userSlice.actions;
export default userSlice.reducer;
