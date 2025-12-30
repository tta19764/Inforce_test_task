import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import './index.css';
import UrlsTablePage from './pages/UrlsTablePage.tsx';
import { Provider } from 'react-redux';
import { store } from './store.ts';
import App from './App.tsx';
import ShortUrlInfoPage from './pages/ShortUrlInfoPage.tsx';
import 'bootstrap/dist/css/bootstrap.min.css';
import { userStoreLocalStorage } from './services/userStoreLocalStorage.ts';
import type { IUserStorage } from './interfaces/IUserStorage.ts';
import type { IUrlsService } from './interfaces/IUrlsService.ts';
import { urlsServiceDotNet } from './services/urlsServiceDotNet.ts';

export const userStorage : IUserStorage = userStoreLocalStorage;
export const urlsService : IUrlsService = urlsServiceDotNet;

const router = createBrowserRouter([
  {
    path: '/',
    element: <App />,
    children: [
      {
        path: '/urls',
        element: <UrlsTablePage />
      },
      {
        path: '/urls/:page',
        element: <UrlsTablePage />
      },
      {
        path: '/url/:urlId',
        element: <ShortUrlInfoPage />
      },
    ]
  },
])

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <Provider store={store}>
      <RouterProvider router={router} />
    </Provider>
  </StrictMode>,
);
