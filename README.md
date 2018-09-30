# Getting Started

# Create Service Account Key
export GOOGLE_CLOUD_PROJECT=$(gcloud config get-value core/project)
echo $GOOGLE_CLOUD_PROJECT
gcloud iam service-accounts create my-bigquery-sa --display-name "my bigquery codelab service account"
gcloud iam service-accounts keys create ~/key.json --iam-account  my-bigquery-sa@${GOOGLE_CLOUD_PROJECT}.iam.gserviceaccount.com
gcloud projects add-iam-policy-binding ${GOOGLE_CLOUD_PROJECT} --member "serviceAccount:my-bigquery-sa@${GOOGLE_CLOUD_PROJECT}.iam.gserviceaccount.com" --role "roles/bigquery.user"

# Set GOOGLE_APPLICATION_CREDENTIALS
export GOOGLE_APPLICATION_CREDENTIALS="/home/${USER}/key.json"

# set EXPORTS
source setup.sh

# run
dotnet run


# Copy from common gsutil storage bucket

gsutil cp gs://cloud-samples-data/bigquery/us-states/us-states.json .
