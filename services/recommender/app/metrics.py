from prometheus_client import Counter, Histogram, generate_latest, CONTENT_TYPE_LATEST
from fastapi import Request, Response
from starlette.middleware.base import BaseHTTPMiddleware
import time

# Define Prometheus metrics
REQUEST_COUNT = Counter("recommendation_requests_total", "Total recommendation requests", ["endpoint"])
RESPONSE_TIME = Histogram("recommendation_response_time_seconds", "Response time per endpoint", ["endpoint"])

# Middleware to measure each request
class MetricsMiddleware(BaseHTTPMiddleware):
    async def dispatch(self, request: Request, call_next):
        start_time = time.time()
        response = await call_next(request)
        duration = time.time() - start_time
        endpoint = request.url.path

        REQUEST_COUNT.labels(endpoint=endpoint).inc()
        RESPONSE_TIME.labels(endpoint=endpoint).observe(duration)

        return response

# Setup function to attach to FastAPI app
def setup_metrics(app):
    app.add_middleware(MetricsMiddleware)

    @app.get("/metrics")
    async def metrics():
        return Response(content=generate_latest(), media_type=CONTENT_TYPE_LATEST)