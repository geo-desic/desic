import { defineConfig, loadEnv } from 'vite';
import plugin from '@vitejs/plugin-react';

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
    const env = loadEnv(mode, process.cwd(), '');

    return {
        plugins: [plugin()],
        server: {
            port: Number.parseInt(env.VITE_PORT),
            proxy: {
                '/api': {
                    target: process.env.services__api__https__0 || process.env.services__api__http__0,
                    changeOrigin: true,
                    secure: false,
                    rewrite: (path) => path.replace(/^\/api/, '')
                }
            }
        }
    }
})
