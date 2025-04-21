from fastapi import FastAPI
from app.routes import router

app = FastAPI(
    title="We Are Cooking - Recommender System",
    version="1.0.0"
)

app.include_router(router)

# Attach Prometheus metrics
try:
    from app.metrics import setup_metrics
    setup_metrics(app)
except ImportError:
    pass
