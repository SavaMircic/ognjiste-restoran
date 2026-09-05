import { registerLocaleData } from '@angular/common';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import localeSr from '@angular/common/locales/sr-Latn';
import { ApplicationConfig, LOCALE_ID, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, withComponentInputBinding, withInMemoryScrolling } from '@angular/router';

import { authInterceptor } from './core/interceptori/auth.interceptor';
import { greskeInterceptor } from './core/interceptori/greske.interceptor';
import { routes } from './app.routes';

registerLocaleData(localeSr);

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    { provide: LOCALE_ID, useValue: 'sr-Latn' },

    provideRouter(
      routes,
      withInMemoryScrolling({ scrollPositionRestoration: 'top', anchorScrolling: 'enabled' }),
      withComponentInputBinding(),
    ),

    provideHttpClient(withInterceptors([greskeInterceptor, authInterceptor])),
    provideAnimationsAsync(),
  ],
};
