FROM node:25-slim AS development

WORKDIR /app

# Set non-root user for security (included in node image)
#RUN chown -R node:node /app

COPY package*.json ./
#COPY --chown=node:node package*.json ./

RUN ls -F ./

RUN npm install

# Copy source files (excluding those in .dockerignore)
#COPY --chown=node:node . .

EXPOSE 56054

# Use non-root user
#USER node

CMD ["npm", "run", "dev"]