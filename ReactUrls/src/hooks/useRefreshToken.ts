import axios from '../api/axios';
import { updateToken } from '../features/user/UserSlice';
import { useAppDispatch } from './reduxHooks';

const useRefreshToken = () => {
    const dispatch = useAppDispatch();

    const refresh = async () => {
        const response = await axios.get('/refresh-token', {
            withCredentials: true
        });
        dispatch(updateToken(response.data));
        return response.data.accessToken;
    }
    return refresh;
};

export default useRefreshToken;
