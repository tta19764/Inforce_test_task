import { configureStore } from "@reduxjs/toolkit";
import userReducer from './features/user/UserSlice';
import modalReducer from './features/modal/modalSlice'


export const store = configureStore({
    reducer: {
        user: userReducer,
        modal: modalReducer,
    }
});

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;