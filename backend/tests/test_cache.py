import pytest
from backend.cache import ResponseCache
import time

def test_cache_set_get():
    cache = ResponseCache(max_size=2)
    cache.set("k1", "v1")
    assert cache.get("k1") == "v1"

def test_cache_expiration():
    cache = ResponseCache(ttl=0.1)
    cache.set("k1", "v1")
    time.sleep(0.2)
    assert cache.get("k1") is None

def test_cache_lru():
    cache = ResponseCache(max_size=2)
    cache.set("k1", "v1")
    cache.set("k2", "v2")
    cache.set("k3", "v3")
    assert cache.get("k1") is None
    assert cache.get("k2") == "v2"
    assert cache.get("k3") == "v3"
