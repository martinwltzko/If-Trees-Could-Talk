from django.utils.deprecation import MiddlewareMixin
from .models import AuthToken


class TokenRenewalMiddleware(MiddlewareMixin):
    def process_request(self, request):
        token = request.META.get('HTTP_AUTHORIZATION', '').split()
        if len(token) == 2 and token[0].lower() == 'token':
            try:
                auth_token = AuthToken.objects.get(token=token[1])
                if auth_token.is_expired():
                    # Token is expired, delete it
                    auth_token.delete()
                else:
                    # Optionally renew the token if it's close to expiry
                    auth_token.renew()
            except AuthToken.DoesNotExist:
                pass
