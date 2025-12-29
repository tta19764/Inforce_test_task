import { createSlice } from "@reduxjs/toolkit";

const initialState = {
    isOpen: false,
    updateTable: true,
};

const modalSlice = createSlice({
    name: "modal",
    initialState,
    reducers: {
        openModal: (state) => {
            state.isOpen = true;
        },
        closeModal: (state) => {
            state.isOpen = false;
        },
        setUpdateTable: (state) => {
            state.updateTable = !state.updateTable;
        }
    },
});

export const { openModal, closeModal, setUpdateTable } = modalSlice.actions;
export default modalSlice.reducer;