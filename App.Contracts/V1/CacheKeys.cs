using System;
using System.Collections.Generic;
using System.Text;

namespace Banking.Contracts.V1
{
    public static class CacheKeys
    {
        public const string AuthSettings = "authsettings";
        public const string central_authSettings = "central_authsettings";
        public const string other_service_authSettings = "other_service_authsettings";
        public const string parent_activities = "parent_activities";
        public const string all_staff = "all_staff";
        public const string all_roles = "all_roles";
        public const string all_modules = "all_modules";
        public const string all_activities = "all_activities";
        public const string all_questions = "all_questions";
        public const string per_user_roles = "per_user_roles";

        public const string common_branchies = "common_branchies";
        public const string common_cities = "common_cities";
        public const string common_currency_rate = "common_currency_rate";
        public const string common_identifications = "common_identifications";
        public const string common_currencies = "common_currencies";
        public const string common_countries = "common_countries";
        public const string common_document_types = "common_document_types";
        public const string common_states = "common_states";
        public const string common_titles = "common_titles";
        public const string common_job_titles = "common_job_titles";
    }
}
