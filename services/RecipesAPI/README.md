# Recipes API for We Are Cooking web application

## Recipes API is a .NET 8 WebAPI project.
This project is a full web API on the .NET 8 platform.

## Configuration
It is configured so the docker-compose file located in ../infrastructure.

## What it does
After setup of the database, the WebAPI performs a check to find out if the ingredients table is filled, if not, the seeding begins.

Two controllers are implemented as of now:
### RecipesController
CRUD for recipes.
/recipesapi/recipes

### IngredientsController
CRUD for ingredients.
/recipesapi/ingredients

## Access
The application by default is accessible on port 7141 for HTTPS and 7140 for HTTP. The Swagger UI is open for use in the standard view.
