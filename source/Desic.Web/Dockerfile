FROM node:25-slim

WORKDIR /app

COPY package*.json ./

RUN npm ci

# Copy source files (excluding those in .dockerignore)
COPY . .

EXPOSE 56054

# Set non-root user for security (included in node image)
RUN chown -R node:node /app
# Use non-root user
USER node

CMD ["npm", "run", "dev"]