"""app URL Configuration

The `urlpatterns` list routes URLs to views. For more information please see:
    https://docs.djangoproject.com/en/4.0/topics/http/urls/
Examples:
Function views
    1. Add an import:  from my_app import views
    2. Add a URL to urlpatterns:  path('', views.home, name='home')
Class-based views
    1. Add an import:  from other_app.views import Home
    2. Add a URL to urlpatterns:  path('', Home.as_view(), name='home')
Including another URLconf
    1. Import the include() function: from django.urls import include, path
    2. Add a URL to urlpatterns:  path('blog/', include('blog.urls'))
"""
from django.contrib import admin
from django.urls import include, path
from dotenv import load_dotenv
from pathlib import Path
import os

# Your custom token model
# Get the parent directory of the current working directory
parent_dir = Path(__file__).resolve().parent.parent
env_path = parent_dir / '.env'
# Load environment variables
load_dotenv(dotenv_path=env_path)

urlpatterns = [
    path(os.environ.get('ADMIN_PATH'), admin.site.urls),
    path(r'api/', include('service.urls')),
    path('api-auth/', include('rest_framework.urls', namespace='rest_framework'))
]
