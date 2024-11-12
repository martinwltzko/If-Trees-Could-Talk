from rest_framework import permissions
from dotenv import load_dotenv
from pathlib import Path
import requests
import os

# Get the parent directory of the current working directory
parent_dir = Path(__file__).resolve().parent.parent
env_path = parent_dir / '.env'
# Load environment variables
load_dotenv(dotenv_path=env_path)


class IsOwnerOrReadOnly(permissions.BasePermission):
    """
    Custom permission to only allow owners of an object to edit it.
    """
    def has_object_permission(self, request, view, obj):
        # Read permissions are allowed to any request,
        # so we'll always allow GET, HEAD or OPTIONS requests.
        if request.method in permissions.SAFE_METHODS:
            return True

        print(f"Checking for {request.data}")

        # Write permissions are only allowed to the owner of the chunk.
        print(f"Checking if {obj.owner} == {request.user}")
        return str(obj.owner) == str(request.user)