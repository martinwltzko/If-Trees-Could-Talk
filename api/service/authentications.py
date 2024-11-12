from rest_framework.authentication import BaseAuthentication
from rest_framework.exceptions import AuthenticationFailed
from django.contrib.auth.models import User
from django.utils import timezone
from datetime import timedelta
from pathlib import Path
from dotenv import load_dotenv
from .models import AuthToken
import requests
import os

# Your custom token model
# Get the parent directory of the current working directory
parent_dir = Path(__file__).resolve().parent.parent
env_path = parent_dir / '.env'
# Load environment variables
load_dotenv(dotenv_path=env_path)


def create_new_token(user):
    import uuid
    token = str(uuid.uuid4())
    expires = timezone.now() + timedelta(hours=1)  # Example: 1-hour expiration

    # Check if a token already exists for the user
    auth_token, created = AuthToken.objects.update_or_create(
        user=user,
        defaults={'token': token, 'expires': expires, 'player_id': user.username},
    )

    return auth_token.token


class UnityAuthentication(BaseAuthentication):
    def authenticate(self, request):
        if 'HTTP_AUTHORIZATION' not in request.META:
            return None

        auth = request.META['HTTP_AUTHORIZATION'].split()
        if len(auth) != 2 or auth[0].lower() != "token":
            return None

        token = auth[1]

        # Check if the token exists and is not expired
        try:
            auth_token = AuthToken.objects.get(token=token)
            if auth_token.is_expired():
                # Optionally, renew the token if it's close to expiry
                auth_token.renew()
            return auth_token.user, None
        except AuthToken.DoesNotExist:
            # Token doesn't exist in the database
            pass

        # Token not found in the database, perform Unity authentication
        player_id = request.data.get('playerId', None)
        if not player_id:
            raise AuthenticationFailed("No playerId provided")

        player_name = request.data.get('playerName', None)
        if not player_name:
            player_name = player_id

        # Validate token with Unity authentication service
        url = f'https://player-auth.services.api.unity.com/v1/users/{player_id}'
        headers = {
            'Authorization': f'Bearer {token}',
            'ProjectId': os.environ.get('UNITY_PROJECT_ID'),
        }
        response = requests.get(url, headers=headers)

        if response.status_code != 200 or response.json().get('id') != player_id:
            raise AuthenticationFailed("Invalid Unity credentials")

        # Create a new AuthToken for the user upon successful Unity authentication
        user, created = User.objects.get_or_create(username=player_id) # Implement this to create a Django user if needed
        new_token = create_new_token(user)
        print(f"New token created for {player_id}:{new_token}")

        return user, None

