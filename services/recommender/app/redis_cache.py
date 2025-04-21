import redis
import os
import json

# Redis config from environment variables
redis_host = os.getenv("REDIS_HOST", "localhost")
redis_port = int(os.getenv("REDIS_PORT", 6379))
redis_db = int(os.getenv("REDIS_DB", 0))

# Connect to Redis
r = redis.Redis(host=redis_host, port=redis_port, db=redis_db, decode_responses=True)

def get_cached_recommendations(user_id: str, rec_type: str = "default"):
    key = f"recommend:{rec_type}:{user_id}"
    data = r.get(key)
    return json.loads(data) if data else None

def cache_recommendations(user_id: str, recs: list, rec_type: str = "default", ttl: int = 300):
    key = f"recommend:{rec_type}:{user_id}"
    r.set(key, json.dumps(recs), ex=ttl)