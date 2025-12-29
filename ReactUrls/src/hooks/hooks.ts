import { type TypedUseSelectorHook, useSelector } from "react-redux";
import { useDispatch } from "react-redux";
import type { AppDispatch } from "../store";
import type { RootState } from "../store";

export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;
export const useAppDispatch = () => useDispatch<AppDispatch>();