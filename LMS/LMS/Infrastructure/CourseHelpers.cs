using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using LMS.Models;
using System.Data.Entity;
using System.Text;

namespace LMS.Infrastructure
{
    /*enum with search types*/
    public enum SearchBy : int
    {
        Name,
        Tag,
        All
    };

  
    /*course class with searching methods*/
    sealed public class CourseSearchHelper
    {
        public static Int32 ItemsOnPageCount = 6;

        public Int32 ItemsCount = 0;
        private static Int32 minimalStringLength = 1;
        private static Int32 minimalCourseNameLength = 4;
        private List<Course> foundCourseCache;

        /*
         * search course by current users choise (searchBy)
         * query - users query
         * currentPage - course Page
         * ItemsCount = 0 (all methods with pages returns totalItemsCount)
         */
        public Task<List<Course>> SearchCoursesAsync(Int32 searchBy, String query, ApplicationDbContext appContext, Int32 currentPage)
        {
            try
            {
                ItemsCount = 0;
                switch ((SearchBy)searchBy)
                {
                    case SearchBy.Name:
                        {
                            return GetCoursesByNameAsync(appContext, query, currentPage);
                        }
                    case SearchBy.Tag:
                        {
                            return GetCoursesByTagsAsync(appContext, query, currentPage);
                        }
                    case SearchBy.All:
                        {
                            return SearchAllCoursesAsync(appContext, query, currentPage);
                        }
                    default:
                        {
                            return SearchAllCoursesAsync(appContext, query, currentPage);
                        }
                }
            }
            catch (InvalidCastException)
            {
                return SearchAllCoursesAsync(appContext, query, currentPage);
            }
        }
        /*
         * search course by current users choise (searchBy)
         * query - users query
         * currentPage - course Page
         * ItemsCount = 0 (all methods with pages returns totalItemsCount)
         */
        public  Task<List<Course>> SearchCoursesAsync(String searchBy, String query, ApplicationDbContext appContext, Int32 currentPage)
        {
            try
            {
                ItemsCount = 0;
                switch (searchBy.ToLower())
                {
                    case "names":
                        {
                            return GetCoursesByNameAsync(appContext, query, currentPage);
                        }
                    case "tags":
                        {
                            return GetCoursesByTagsAsync(appContext, query, currentPage);
                        }
                    case "all":
                        {
                            //return GetCoursesByCategoryAsync(appContext, query, currentPage);
                            return SearchAllCoursesAsync(appContext, query, currentPage);
                        }
                    default:
                        {
                            return SearchAllCoursesAsync(appContext, query, currentPage);
                        }
                }
            }
            catch (InvalidCastException)
            {
                return SearchAllCoursesAsync(appContext, query, currentPage);
            }
        }
        /*
         * search courses by all paramethers (tags, names) for current page
         */
        public async Task<List<Course>> SearchAllCoursesAsync(ApplicationDbContext appContext, String query, Int32 currentPage)
        {
            /*search courses by all types*/
            try
            {
                if (query.Length >= minimalStringLength)
                {
                    List<Course> foundCourses = new List<Course>();
                    ItemsCount = 0;
                    foundCourses.AddRange(await GetCoursesByNameAsync(appContext, query));
                    foundCourses.AddRange(await GetCoursesByTagsAsync(appContext, query));
                    if (foundCourses.Count != 0)
                    {
                        foundCourseCache = foundCourses
                            .GroupBy(c => c.ID)
                            .Select(c => c.First())
                            .OrderBy(c => c.ID)
                            .ToList();
                        ItemsCount = foundCourseCache.Count;
                   
                        return foundCourseCache.Skip(currentPage * ItemsOnPageCount)
                                    .Take(ItemsOnPageCount)
                                    .ToList();
                    }

                }
                return new List<Course>();
            }
            catch (NullReferenceException)
            {
                return new List<Course>();
            }
        }
        /*
         * returns courses for current page,
         * number of courses depends on static value ItemsOnPageCount
         */
        public static async Task<IList<Course>> GetCoursesOnPageAsync(ApplicationDbContext appContext, Int32 currentPage)
        {
            try
            {
                if (appContext != null)
                {
                    IList<Course> courses = await appContext.Courses
                        .OrderBy(c => c.ID)
                        .Skip(currentPage * CourseSearchHelper.ItemsOnPageCount)
                        .Take(CourseSearchHelper.ItemsOnPageCount)
                        .ToArrayAsync();
                    if (courses != null && courses.Count != 0)
                        return courses;
                }
                return new List<Course>();
            }
            catch (NullReferenceException)
            {
                return new List<Course>();
            }
        }
        /*
         * returns courses count for current page with users query tags
         */
        public async Task<List<Course>> GetCoursesByTagsAsync(ApplicationDbContext appContext, String tags, Int32 currentPage)
        {
            try
            {
                if (appContext != null && tags != String.Empty)
                {
                    String[] splitted = GetSplitString(tags);
                    if (splitted.Length != 0)
                    {
                        List<ICollection<Course>> courses = await appContext.CourseTags
                            .Where(i => splitted.Contains(i.Tag.ToLower()))
                            .Select(i => i.Courses)
                            .ToListAsync();
                        if (courses != null)
                        {
                            List<Course> selectedCourses = new List<Course>();
                            for (Int32 i = 0; i < courses.Count; i++)
                            {
                                for (Int32 j = 0; j < (courses[i] == null ? 0 : courses[i].Count); j++)
                                    selectedCourses.Add(courses[i].ElementAt(j));
                            }
                            IEnumerable<Course> groupedCourses = selectedCourses
                                .GroupBy(c => c.ID)
                                .Select(c => c.First());
                            if (groupedCourses != null)
                            {
                                ItemsCount = groupedCourses.Count();
                                selectedCourses = groupedCourses
                                    .OrderBy(i => i.ID)
                                    .Skip(currentPage * ItemsOnPageCount)
                                    .Take(ItemsOnPageCount).ToList();
                                return selectedCourses;
                            }
                        }
                    }
                }
                return new List<Course>();
            }
            catch (NullReferenceException)
            {
                /*can't find tags and courses*/
                return new List<Course>();
            }
        }
        /*public static async Task<List<Course>> GetCoursesByCategoryAsync(ApplicationDbContext appContext, String categories, Int32 currentPage)
        {
            try
            {
                if (appContext != null && categories != String.Empty)
                {
                    String[] splitted = GetSplitString(categories);
                    if (splitted != null)
                    {
                        List<ICollection<Course>> courses = await appContext.CourseCategories
                            .Where(i => splitted.Contains(i.Category.ToLower()))
                            .Select(i => i.Courses)
                            .ToListAsync();
                        if (courses != null)
                        {
                            List<Course> selectedCourses = new List<Course>();
                            for (Int32 i = 0; i < courses.Count; i++)
                            {
                                for (Int32 j = 0; j < (courses[i] == null ? 0 : courses[i].Count); j++)
                                    selectedCourses.Add(courses[i].ElementAt(j));
                            }
                            ItemsCount = selectedCourses.Count;
                            selectedCourses = selectedCourses
                                .Skip(currentPage * ItemsOnPageCount)
                                .Take(ItemsOnPageCount).ToList();
                            return selectedCourses;
                        }
                    }
                }
                return new List<Course>();
            }
            catch (NullReferenceException)
            {
                return new List<Course>();
            }
        }
        */
        /*
         * returns list of courses where count = currentpage courses count 
         */
        public async Task<List<Course>> GetCoursesByNameAsync(ApplicationDbContext appContext, String name, Int32 currentPage)
        {
            try
            {
                if (appContext != null && name != String.Empty)
                {
                    String[] splitted = GetSplitString(name);
                    if (splitted.Length != 0)
                    {
                        List<Course> courses = new List<Course>();
                        for (Int32 i = 0; i < splitted.Length; i++)
                        {
                            if (splitted[i].Length >= minimalCourseNameLength)
                            {
                                String splitElement = splitted[i];
                                courses.AddRange(
                                    await appContext.Courses
                                    .Where(course => course.Title.ToLower().Contains(splitElement))
                                    .ToListAsync()
                                    );
                            }
                        }
                        var coursesSaver = courses
                            .GroupBy(course => course.ID)
                            .Select(cGroup => cGroup.First());
                        if (coursesSaver != null)
                        {
                            ItemsCount = coursesSaver.Count();
                            courses = coursesSaver
                                .Skip(currentPage * ItemsOnPageCount)
                                .Take(ItemsOnPageCount)
                                .ToList();
                        }
                        return courses;
                    }
                }
                return new List<Course>();
            }
            catch (NullReferenceException)
            {
                return new List<Course>();
            }
            catch (ArgumentNullException)
            {
                return new List<Course>();
            }
        }
        
        /*without pages*/
        /*
         * search course by current users choise (searchBy)
         * query - users query
         */
        public  Task<List<Course>> SearchCoursesAsync(Int32 searchBy, String query, ApplicationDbContext appContext)
        {
            try
            {
                ItemsCount = 0;
                switch ((SearchBy)searchBy)
                {
                    case SearchBy.Name:
                        {
                            return GetCoursesByNameAsync(appContext, query);
                        }
                    case SearchBy.Tag:
                        {
                            return GetCoursesByTagsAsync(appContext, query);
                        }
                    case SearchBy.All:
                        {                           
                            return SearchAllCoursesAsync(appContext, query);
                        }
                    default:
                        {
                            return SearchAllCoursesAsync(appContext, query);
                        }
                }
            }
            catch (InvalidCastException)
            {
                return SearchAllCoursesAsync(appContext, query);
            }
        }
        /*
         * search course by current users choise (searchBy)
         * query - users query
        */
        public Task<List<Course>> SearchCoursesAsync(String searchBy, String query, ApplicationDbContext appContext)
        {
            try
            {
                ItemsCount = 0;
                switch (searchBy.ToLower())
                {
                    case "names":
                        {
                            return GetCoursesByNameAsync(appContext, query);
                        }
                    case "tags":
                        {
                            return GetCoursesByTagsAsync(appContext, query);
                        }
                    case "all":
                        {
                            //return GetCoursesByCategoryAsync(appContext, query, currentPage);
                            return SearchAllCoursesAsync(appContext, query);
                        }
                    default:
                        {
                            return SearchAllCoursesAsync(appContext, query);
                        }
                }
            }
            catch (InvalidCastException)
            {
                return SearchAllCoursesAsync(appContext, query);
            }
        }
        /*
         * returns courses tags from users query(tags)
         * if tags[i]==course.Tag[j] OK
         * else NOT OK
         */
        public static async Task<List<Course>> GetCoursesByTagsAsync(ApplicationDbContext appContext, String tags)
        {
            try
            {
                if (appContext != null && tags != String.Empty)
                {
                    String[] splitted = GetSplitString(tags);
                    if (splitted.Length != 0)
                    {
                        List<ICollection<Course>> courses = await appContext.CourseTags
                            .Where(i => splitted.Contains(i.Tag.ToLower()))
                            .Select(i => i.Courses)
                            .ToListAsync();
                        if (courses != null)
                        {
                            List<Course> selectedCourses = new List<Course>();
                            for (Int32 i = 0; i < courses.Count; i++)
                            {
                                for (Int32 j = 0; j < (courses[i] == null ? 0 : courses[i].Count); j++)
                                    selectedCourses.Add(courses[i].ElementAt(j));
                            }
                            selectedCourses = selectedCourses
                                .GroupBy(c => c.ID)
                                .Select(group => group.First())
                                .ToList();
                            return selectedCourses;
                        }
                    }
                }
                return new List<Course>();
            }
            catch (NullReferenceException)
            {
                /*can't find tags and courses*/
                return new List<Course>();
            }
        }
        /*public static async Task<List<Course>> GetCoursesByCategoryAsync(ApplicationDbContext appContext, String categories)
        {
            try
            {
                if (appContext != null && categories != String.Empty)
                {
                    String[] splitted = GetSplitString(categories);
                    if (splitted != null)
                    {
                        List<ICollection<Course>> courses = await appContext.CourseCategories
                            .Where(i => splitted.Contains(i.Category.ToLower()))
                            .Select(i => i.Courses)
                            .ToListAsync();
                        if (courses != null)
                        {
                            List<Course> selectedCourses = new List<Course>();
                            for (Int32 i = 0; i < courses.Count; i++)
                            {
                                for (Int32 j = 0; j < (courses[i] == null ? 0 : courses[i].Count); j++)
                                    selectedCourses.Add(courses[i].ElementAt(j));
                            }
                            return selectedCourses;
                        }
                    }
                }
                return new List<Course>();
            }
            catch (NullReferenceException)
            {
                return new List<Course>();
            }
        }*/
        /*
         * Search courses by names
         * if  courses name contains *name then true
         * else false
         */
        public static async Task<List<Course>> GetCoursesByNameAsync(ApplicationDbContext appContext, String name)
        {
            try
            {
                if (appContext != null && name != String.Empty)
                {
                    String[] splitted = GetSplitString(name);
                    if (splitted.Length != 0)
                    {
                        List<Course> courses = new List<Course>();
                        for (Int32 i = 0; i < splitted.Length; i++)
                        {
                            if (splitted[i].Length >= minimalCourseNameLength)
                            {
                                String splitElement = splitted[i];
                                courses.AddRange(
                                    await appContext.Courses
                                    .Where(course => course.Title.ToLower().Contains(splitElement))
                                    .ToListAsync()
                                    );
                            }
                        }
                        courses = courses
                            .GroupBy(course => course.ID)
                            .Select(cGroup => cGroup.First())
                            .ToList();
                        return courses;
                    }
                }
                return new List<Course>();
            }
            catch (NullReferenceException)
            {
                return new List<Course>();
            }
            catch (ArgumentNullException)
            {
                return new List<Course>();
            }
        }
        /*
         * search course by names and tags from users query
         */
        public async Task<List<Course>> SearchAllCoursesAsync(ApplicationDbContext appContext, String query)
        {
            try
            {
                if (query.Length >= minimalStringLength)
                {
                    List<Course> foundCourses = new List<Course>();
                    foundCourses.AddRange(await GetCoursesByNameAsync(appContext, query));
                    foundCourses.AddRange(await GetCoursesByTagsAsync(appContext, query));
                    if (foundCourses.Count != 0)
                    {
                        foundCourseCache = foundCourses
                            .GroupBy(c => c.ID)
                            .Select(c => c.First())
                            .OrderBy(c => c.ID)
                            .ToList();
                        ItemsCount = foundCourseCache.Count;

                        return foundCourseCache;
                    }
                }
                return new List<Course>();
            }
            catch (NullReferenceException)
            {
                return new List<Course>();
            }
        }
        /*
         * edits users query
         * returns array of tags or names from users query
         */
        private static String[] GetSplitString(String tags)
        {
            if (tags != null && tags.Length != 0)
            {
                String[] splitted = null;
                tags = tags.Trim().ToLower();
                if (tags.Contains(' ') || tags.Contains(',') || tags.Contains('.'))
                    splitted = tags.Split(new Char[] { ' ', ',', '.', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (splitted != null && splitted.Length != 0)
                    return splitted;
                return new String[] { tags };
            }
            return new String[] { };
        }
    }




    public static class UserHelper
    {
        private static String[] AvaibleImageExtensions = { ".jpg", ".jpeg", ".bmp", ".gif", ".png" };
        private static String defaultImagePath = "/Content/img/fff.png";




        /*USABLE METHODS*/

        /*
         * Saving course file (image from local computer or by url)
         * file - file from users local PC
         * fileSite - url of image 
         * course - course to save image
         * serverMapPath - path on server to save image:
         * for example:
         * serverMapPAth - C://aaaa//aaa//
         * image new path - C://aaaa//aaa//Course{ID}//Course{ID}.extention
         */
        public static Boolean SavePersonalFile(HttpPostedFileBase file, String fileSite, ApplicationUserInfo appuser, String serverMapPath, String fileNameBegin = "AppUser", Boolean save = true)
        {
            if (file != null)
            {
                String fileName = fileNameBegin + appuser.Id.ToString();
                String imagePath = SaveImageToFolder(fileName, file, serverMapPath);
                if (imagePath != String.Empty)
                {
                    appuser.Image = imagePath;
                    return true;
                }
            }
            ///*saving images http
            else if (fileSite != String.Empty)
            {
                if (UserHelper.ValidateHttpImageRoute(fileSite))
                {
                    appuser.Image = fileSite;
                    return true;
                }
            }
            else if (save)
            {
                appuser.Image = defaultImagePath;
                return true;
            }


            /*saving image is not main task*/
            if (!save)
                return true;
            return false;
        }

        /*
         * returns created image path
         * first - saving image
         * then returns saved file path
         */
        public static String SaveImageToFolder(String fileName, HttpPostedFileBase file, String dirPath)
        {
            if (fileName != null && file != null && dirPath != null)
            {
                String path = Path.Combine(dirPath, fileName);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                String imagePath = Path.Combine(path, fileName + Path.GetExtension(file.FileName));

                file.SaveAs(imagePath);

                String createdPath = String.Format("/Content/img/User/{0}/{1}", fileName, fileName + Path.GetExtension(file.FileName));

                return createdPath;
            }
            return String.Empty;
        }

        public static Boolean ValidateHttpImageRoute(String httpPath)
        {
            /*searching for */
            if (Path.HasExtension(httpPath))
            {
                if (AvaibleImageExtensions.Contains(Path.GetExtension(httpPath).ToLower()))
                {
                    if (httpPath.IndexOf("http://") == 0 || httpPath.IndexOf("https://") == 0)
                        return true;
                }
            }
            return false;
        }

    }


    public static class CourseHelper
    {
        private static String[] AvaibleImageExtensions = {".jpg", ".jpeg", ".bmp", ".gif", ".png" };
        private static String defaultImagePath = "/images/easyStudy.jpg";

       
        
        
        /*USABLE METHODS*/

        /*
         * Saving course file (image from local computer or by url)
         * file - file from users local PC
         * fileSite - url of image 
         * course - course to save image
         * serverMapPath - path on server to save image:
         * for example:
         * serverMapPAth - C://aaaa//aaa//
         * image new path - C://aaaa//aaa//Course{ID}//Course{ID}.extention
         */
        public static Boolean SaveCourseFile(HttpPostedFileBase file, String fileSite, Course course, String serverMapPath, String fileNameBegin = "Course", Boolean save = true)
        {
            if (file != null)
            {
                String fileName = fileNameBegin + course.ID.ToString();
                String imagePath = SaveImagePersonToFolder(fileName, file, serverMapPath);
                if (imagePath != String.Empty)
                {
                    course.Image = imagePath;
                    return true;
                }
            }
            ///*saving images http
            else if (fileSite != String.Empty)
            {
                if (CourseHelper.ValidateHttpImageRoute(fileSite))
                {
                    course.Image = fileSite;
                    return true;
                }
            }
            else if (save)
            {
                course.Image = defaultImagePath;
                return true;
            }


            /*saving image is not main task*/
            if (!save)
                return true;
            return false;
        }

        /*
         * returns created image path
         * first - saving image
         * then returns saved file path
         */
        public static String SaveImagePersonToFolder(String fileName, HttpPostedFileBase file, String dirPath)
        {
            if (fileName != null && file != null && dirPath != null)
            {
                String path = Path.Combine(dirPath, fileName);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                String imagePath = Path.Combine(path, fileName + Path.GetExtension(file.FileName));

                file.SaveAs(imagePath);

                String createdPath = String.Format("/Content/img/Courses/{0}/{1}", fileName, fileName + Path.GetExtension(file.FileName));

                return createdPath;
            }
            return String.Empty;
        }
        /*
         * returns parsed IDs from ids string with defined separator
         * count of IDs - all,0, all parse succeeded
         */
        private static IList<Int32> ParseStringToIdVector(String tags, Char separator)
        {
            List<Int32> ids = new List<Int32>();
            try
            {
                String[] splitIds = tags.Split(separator);
                if (splitIds != null && splitIds.Length != 0)
                {
                    for (Int32 i = 0; i < splitIds.Length; i++)
                        ids.Add(Int32.Parse(splitIds[i]));
                    return ids;
                }
                return new List<Int32>();
            }
            catch (FormatException)
            {
                if (ids.Count != 0)
                    return ids;
                else return new List<Int32>();
            }
            catch (InvalidCastException)
            {
                if (ids.Count != 0)
                    return ids;
                else return new List<Int32>();
            }
        }


        /*
         * validates HTTP route
         * if contains required extensions and http:// or https:// then true
         * else false
         */
        public static Boolean ValidateHttpImageRoute(String httpPath)
        {
            /*searching for */
            if (Path.HasExtension(httpPath))
            {
                if (AvaibleImageExtensions.Contains(Path.GetExtension(httpPath).ToLower()))
                {
                    if (httpPath.IndexOf("http://") == 0 || httpPath.IndexOf("https://") == 0)
                        return true;
                }
            }
            return false;
        }
        
        

        
        /*
         * Initializes all courses collections (next method with collection of progresses)
         */
        public static Task InitializeAllCourseComponentsAsync(Course course, ApplicationDbContext appContext)
        {
            return Task.Factory.StartNew(() =>
                {
                    if (!appContext.Entry(course).Collection(c => c.Groups).IsLoaded)
                        appContext.Entry(course).Collection(c => c.Groups).Load();
                    if (!appContext.Entry(course).Collection(c => c.Topics).IsLoaded)
                        appContext.Entry(course).Collection(c => c.Topics).Load();
                    if (!appContext.Entry(course).Collection(c => c.CourseTags).IsLoaded)
                        appContext.Entry(course).Collection(c => c.CourseTags).Load();
                });
        }
        /*
         * Initializes courses cshedules, topics and tags in new thread
         */
        public static Task InitializeCourseDetailsComponentsAsync(Course course, ApplicationDbContext appContext)
        {
            return Task.Factory.StartNew(() =>
            {
                if (!appContext.Entry(course).Collection(c => c.Topics).IsLoaded)
                    appContext.Entry(course).Collection(c => c.Topics).Load();
                if (!appContext.Entry(course).Collection(c => c.CourseTags).IsLoaded)
                    appContext.Entry(course).Collection(c => c.CourseTags).Load();
            });
        }
        /*
         * returns IList of course tags from Vector of course IDs
         * Vector<Int32> => Vector<CourseTags>
         */
        public static async Task<IList<CourseTag>> GetTagsFromIdsAsync(IEnumerable<Int32> tagIds, ApplicationDbContext appContext)
        {
            if (tagIds != null && tagIds.Count() != 0 && appContext != null)
            {
                List<CourseTag> tagList = new List<CourseTag>();
                CourseTag newTag = null;
                foreach (Int32 i in tagIds)
                {
                    newTag = await appContext.CourseTags.FindAsync(i);
                    if (newTag != null)
                        tagList.Add(newTag);
                }
                return tagList;
            }
            return new List<CourseTag>();
        }

        public static Boolean GetTagsFromStringOrCreate(String names, ApplicationDbContext appContext, ref IList<CourseTag> tags, Char separator = ',', Boolean create = true)
        {
            Boolean result = false;
            if (names != null && names != String.Empty && appContext != null)
            {
                String[] splitTags = names.Split(separator);
                if (splitTags != null && splitTags.Length != 0)
                {
                    List<CourseTag> courseTags = new List<CourseTag>();
                    String tag = null;
                    for (Int32 i = 0; i < splitTags.Length; i++)
                    {
                        tag = splitTags[i];
                        if (appContext.CourseTags.Any(ct => String.Compare(ct.Tag, tag, true) == 0))
                            courseTags.Add(appContext.CourseTags.First(ct => String.Compare(ct.Tag, tag, true) == 0));
                        else if (create)
                        {
                            CourseTag newTag = new CourseTag { Tag = tag };
                            appContext.CourseTags.Add(newTag);
                            courseTags.Add(newTag);
                            if (!result)
                                result = true;
                        }
                    }
                    tags = courseTags;
                }
            }
            return result;
        }
        /*
         * returns List of course tags form string with defined separator
         */
        public static async Task<IList<CourseTag>> GetTagsFormStringAsync(String tags, ApplicationDbContext appContext, Char separator = ',')
        {
            if (tags != null && tags != String.Empty)
               return await GetTagsFromIdsAsync(ParseStringToIdVector(tags, separator), appContext);
            return new List<CourseTag>();
        }
        public static IEnumerable<Int32> GetIntTagsFormStringAsync(String tags, ApplicationDbContext appContext, Char separator = ',')
        {
            if (tags != null && tags != String.Empty)
                return ParseStringToIdVector(tags, separator);
            return new Int32[] { };
        }
        /*
         * removes all  tags from current course
         */
        public static void RemoveTagsFromCourse(Course course)
        {
            if (course != null && course.CourseTags != null && course.CourseTags.Count != 0)
            {
                for (Int32 i = 0; i < course.CourseTags.Count; i++)
                    course.CourseTags.Remove(course.CourseTags.ElementAt(i));
            }
        }
    }
}