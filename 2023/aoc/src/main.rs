use std::fs;
use std::collections::HashMap;
use std::cmp::{Ord, Ordering};

fn main() {
    // day1();
    // day2();
    // day3();
    // day4();
    // day5();
    // day5_2();
    // day6();
    day7();
}

// todo move to mod
fn day1() {
    let contents = fs::read_to_string("./inputs/day1").expect("Should have read the file");
    let list = contents.split("\n");

    let nums = HashMap::from([
        ("1", "1"),
        ("2", "2"),
        ("3", "3"),
        ("4", "4"),
        ("5", "5"),
        ("6", "6"),
        ("7", "7"),
        ("8", "8"),
        ("9", "9"),
        ("one", "1"),
        ("two", "2"),
        ("three", "3"),
        ("four", "4"),
        ("five", "5"),
        ("six", "6"),
        ("seven", "7"),
        ("eight", "8"),
        ("nine", "9")
    ]);

    let mut results: [i32; 1000] = [0; 1000];
    let mut i = 0;

    for item in list {
        let mut earliest_first = i32::MAX;
        let mut latest_last = -1;

        let mut pair = ("", "");

        for num in nums.keys() {
            let first = item.find(num).map_or(i32::MAX, |g| {i32::try_from(g).expect("parse first usize to int")});
            let last = item.rfind(num).map_or(-1, |g| {i32::try_from(g).expect("parse last usize to int")});

            if first < earliest_first
            {
                earliest_first = first;
                pair.0 = nums[num];
            }

            if last > latest_last
            {
                latest_last = last;
                pair.1 = nums[num];
            }
        }

        let mut owned_string: String = "".to_owned();
        owned_string.push_str(pair.0);
        owned_string.push_str(pair.1);

        results[i] = owned_string.parse().expect("should've parsed");
        i = i+1;
    }

    let sum: i32 = results.iter().sum();

    println!("{}", sum);
}

fn day2() {
    let contents = fs::read_to_string("./inputs/day2").expect("Should have read the file");
    let input = contents.split("\n");

    let mut game = 1;
    let mut total = 0;
    let mut power_total = 0;

    let num_red = 12;
    let num_green = 13;
    let num_blue = 14;

    for entire_game in input {
        let pulls = entire_game.split("; ");
        
        let mut pulls_vec: Vec<&str> = pulls.collect();

        pulls_vec[0] = &pulls_vec[0].split(": ").last().expect("there should've been a colon in the first bit");

        let mut valid = true;
        let mut min_red = 0;
        let mut min_green = 0;
        let mut min_blue = 0;

        for pull in pulls_vec {
            let individual_ball_result = pull.split(", ");

            for set in individual_ball_result {
                let mut count_and_colour = set.split(" "); // why does `next`` need this to be mut?

                let ball_count = count_and_colour.next().expect("should have a count").parse::<i32>().expect("should've parsed");
                let ball_colour = count_and_colour.next().expect("should have a colour");

                match (ball_count, ball_colour) {
                    (a, "green") => {if a > num_green {valid = false;} if min_green < a { min_green = a;} },
                    (a, "red") => {if a > num_red {valid = false;} if min_red < a { min_red = a;} },
                    (a, "blue") => {if a > num_blue {valid = false;} if min_blue < a { min_blue = a;} },
                    _ => {}
                }
            }
        }
        
        if valid { total += game; }

        power_total += min_blue * min_green * min_red;
        
        game += 1;
    }

    println!("{}", total);
    println!("{}", power_total); 
}

fn day3() {
    let contents = fs::read_to_string("./inputs/day3").expect("Should have read the file");
    let rows = contents.split("\n");

    let mut map: HashMap<(i32, i32), (char, i32)> = HashMap::new();

    let mut j = 0;
    let mut i = 0;

    let numeric_characters = ['1', '2', '3', '4','5','6','7','8','9','0'];

    for row in rows {
        i=0;
        for character in row.chars() {
            if character == '.' { i += 1; continue; }
            
            let to_insert = if numeric_characters.contains(&character) { -1 } else { 0 };

            map.insert((i, j), (character, to_insert));
            i += 1;
        }

        let mut numbers_for_row: Vec<i32> = Vec::<i32>::new();
        for number in row.split(".")
        {
            if number == "" { continue }

            if number.parse::<i32>().is_ok()
            {
                numbers_for_row.push(number.parse::<i32>().expect("should've been ok"));
            }
        }

        let mut number_count = 0;

        for mut p in 0..i {
            if number_count >= numbers_for_row.len()
            {
                break;
            }

            if map.contains_key(&(p, j)) && map[&(p, j)].1 == -1 {
                while map.contains_key(&(p, j)) {
                    map.entry((p, j)).and_modify(|a| {(a.0, numbers_for_row[number_count]);});
                    p+=1;
                }
                number_count += 1;
            }
        }


        j += 1;
    }
    for thing in map {
        if thing.0.1 == 0 {
            println!("{:?}", thing);
        }
    }


}

fn day4() {
    let contents = fs::read_to_string("./inputs/day4").expect("Should have read the file");
    let rows = contents.split("\n");
    let mut total_score = 0;

    let mut card_win_counts = HashMap::<i32, (i32, i32)>::new(); // tuple = (wins, count)
    let mut count = 1;

    for row in rows {
        let dump_first_bit = row.split(": ").last().expect("should've got last");
        let mut winning_and_actual = dump_first_bit.split(" | ");

        let winning = winning_and_actual.next().expect("should have first part");
        let actual = winning_and_actual.next().expect("should have second part");

        let winning_split = winning.split(" ").filter(|&x| !x.is_empty());
        let actual_split = actual.split(" ").filter(|&x| !x.is_empty());
        
        let winning_split_vec: Vec<&str> = winning_split.collect();
        let actual_vec: Vec<&str> = actual_split.collect();

        let mut score = 0;
        let mut win_count = 0;

        for winner in winning_split_vec {
            if actual_vec.contains(&winner)
            {
                win_count += 1;
                score = if score == 0 { 1 } else { score * 2 };
            }
        }

        total_score += score;
        card_win_counts.insert(count, (win_count, 1));
        count += 1;
    }

    let mut total_cards = 0;

    for i in 1..189 {
        if card_win_counts[&i].0 > 0 // wins > 0
        {
            let to_add = card_win_counts[&i].1;
            
            for j in 1..card_win_counts[&i].0 + 1 {
                *card_win_counts.get_mut(&(i+j)).unwrap() = (card_win_counts[&(i+j)].0, card_win_counts[&(i+j)].1 + to_add);
            }
        }
    }

    
    for card in card_win_counts.iter() {
        total_cards += card.1.1;
    }

    println!("{}", total_score);
    println!("{}", total_cards);

}


fn day5() {
    let contents = fs::read_to_string("./inputs/day5").expect("Should have read the file");
    let mut rows = contents.split("\n");

    let mut seeds: Vec<i64> = rows.next().expect("should be ok")
        .split(": ").last().expect("should still be ok")
        .split(" ").map(|x| x.parse::<i64>().expect("should have i64s")).collect();

    let mut mappings = Vec::<(i64, i64, i64)>::new();

    rows.next();
    rows.next(); // skip past the first title;

    loop {
        let nums = match rows.next() {
            Some(a) => a,
            None => break,
        };

        if nums.is_empty()
        {
            let seeds_copy = seeds.clone();
            
            // map the seeds now
            let mut cnt = 0;

            for seed in seeds_copy {
                for dest_src_len in mappings.iter() {
                    if seed > dest_src_len.1 && seed < dest_src_len.1 + dest_src_len.2 {
                        let new_val = seed - dest_src_len.1 + dest_src_len.0;
                        seeds[cnt] = new_val;
                    }
                }
                cnt += 1;
            }

            // clear the vec?
            mappings.clear();

            // skip the next title
            rows.next();
        }
        else
        {
            let ints:Vec<i64> = nums.split(" ").map(|x| x.parse::<i64>().expect("second time should have i64s")).collect();
            mappings.push((ints[0], ints[1], ints[2]));
        }
    }
    seeds.sort();
    println!("{:?}", seeds);
}

fn day5_2()
{
    let contents = fs::read_to_string("./inputs/day5").expect("Should have read the file");
    let mut rows = contents.split("\n");

    let seeds_input: Vec<i64> = rows.next().expect("should be ok")
        .split(": ").last().expect("should still be ok")
        .split(" ").map(|x| x.parse::<i64>().expect("should have i64s")).collect();

    let mut seeds: Vec<(i64, i64)> = Vec::<(i64, i64)>::new();

    for i in 0..seeds_input.len()/2 {
        seeds.push((seeds_input[2*i], seeds_input[2*i] + seeds_input[2*i+1]));
    }

    rows.next();
    rows.next(); // skip past the first title;

    let mut loop_seeds = Vec::<(i64, i64)>::new();

    loop {

        println!("seeds {:?} & loop_seeds {:?}", seeds, loop_seeds);


        let nums = match rows.next() {
            Some(a) => a,
            None => break
        };

        println!("nums {:?}", nums);

        if nums.is_empty() {
            // replace seeds array proper for the next loop

            seeds.retain(|x| {x.0 != -1});

            // println!("loop {:?}", loop_seeds);

            for seed in loop_seeds.clone().iter() {
                seeds.push(*seed);
            }
            rows.next();
            loop_seeds.clear();
        }
        else {
            // cases

            //    -------- DSL
            //      ---    seed

            //    -------- DSL
            //         ------ seed

            //    -------- DSL
            //  -----        seed
            
            //    -------- DSL
            //   -------------  seed

            let dest_src_len:Vec<i64> = nums.split(" ").map(|x| x.parse::<i64>().expect("second time should have i64s")).collect();
            let mut unmapped_seeds: Vec<(i64, i64)> = Vec::<(i64, i64)>::new();

            for seed in seeds.iter_mut() {
                // mapping range completely covers seeds (map directly range for range)
                if seed.0 > dest_src_len[1] && seed.1 < dest_src_len[1] + dest_src_len[2] {
                    loop_seeds.push((seed.0 - dest_src_len[1] + dest_src_len[0], seed.1 - dest_src_len[1] + dest_src_len[0]));
                    *seed = (-1, -1);
                }

                // mapping covers bottom end of seeds (split into two)
                if seed.0 > dest_src_len[1] && seed.0 < dest_src_len[1] + dest_src_len[2] && seed.1 > dest_src_len[1] + dest_src_len[2] {
                    loop_seeds.push((seed.0 - dest_src_len[1] + dest_src_len[0], dest_src_len[0] + dest_src_len[2]));
                    *seed = (dest_src_len[1] + dest_src_len[2], seed.1); 
                }

                // mapping covers top end of seeds (split into two)
                if seed.0 < dest_src_len[1] && seed.1 > dest_src_len[1] && seed.1 < dest_src_len[1] + dest_src_len[2] {
                    loop_seeds.push((dest_src_len[0], dest_src_len[0] + seed.1 - dest_src_len[1]));
                    *seed = (seed.0, dest_src_len[1] - 1) // questionable minus one here
                }

                // seeds cover mapping range entirely (split into three)
                if seed.0 < dest_src_len[1] && seed.1 > dest_src_len[1] + dest_src_len[2] {
                    loop_seeds.push((seed.0 - dest_src_len[1] + dest_src_len[0], seed.1 - dest_src_len[1] + dest_src_len[0]));
                    *seed = (seed.0, dest_src_len[1] - 1);// lower bit
                    unmapped_seeds.push((dest_src_len[1] + dest_src_len[2] + 1, seed.1));// upper bit // dubious +1
                }
            }

            // rejoin the things together
            for thing in unmapped_seeds {
                seeds.push(thing);
            }

            // println!("{:?}, {:?}", loop_seeds.clone(), seeds);
        }

    }

    let mut min = i64::MAX;
    for seed in seeds.iter() {
        if seed.0 < min { min = seed.0 }
    }

    // println!("{:?}", loop_seeds);

    println!("{:?}", min); // 144724436 too high
}

fn day6() {
    // let inputs = [(7, 9), (15, 40), (30, 200)];
    // let inputs = [(52, 426), (94, 1374), (75, 1279), (94, 1216)];
    let inputs: [(i64, i64); 1] = [(52947594, 426137412791216)];
    let mut res = 1;

    for input in inputs {
        let mut min = input.0 / 2; // by observation, holding the button for this time is always a win
        let mut max = input.0 / 2;

        let mut jump_size = min/2;

        loop {
            let test = (input.0 - (min - jump_size)) * (min - jump_size);

            if test > input.1 {
                min = min - jump_size;
                jump_size = min / 2;
            }
            else {
                jump_size /= 2
            }

            if jump_size == 0 {
                break;
            }
        }

        // find the most amount of time to win
        loop {
            let test = (input.0 - (max + jump_size)) * (max + jump_size);

            if test > input.1 {
                max = max + jump_size;
                jump_size = input.0 - (jump_size / 2);
            }
            else {
                jump_size /= 2
            }

            if jump_size == 0 { // we must have tried jump_size == 1, and divided it by a huge number so that it's now 0, so this answer is correct
                break;
            }
        }

        println!("{}: {}", input.0, max - min + 1); // +1 because it's inclusive 
        res *= max - min + 1;
    }

    println!("{}", res);
}

fn day7() {
    let contents = fs::read_to_string("./inputs/day7").expect("Should have read the file");
    let rows = contents.split("\n");

    let mut hands = rows.map(|x| { 
        let mut y = x.split(" ");
        return (y.next().expect("should have next1"), y.next().expect("should have next2").parse::<i32>().expect("should've parsed"))
    }).collect::<Vec<(&str, i32)>>();

    let mut cards = vec!['A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2'];

    hands.sort_by(|a, b| {
        let first = a.0;
        let second = b.0;

        let mut counts_first = HashMap::<char, i32>::new();
        let mut counts_second = HashMap::<char, i32>::new();

        for card in cards.iter() {
            let mut count_first = 0;
            for c in first.chars() {
                if c == *card {
                    count_first += 1;
                }
            }

            counts_first.insert(*card, count_first);
            
            let mut count_second = 0;
            for c in second.chars() {
                if c == *card {
                    count_second += 1;
                }
            }

            counts_second.insert(*card, count_second);
        }

        // compare on card counts
        let first_max = counts_first.values().filter(|x| {x != &&0}).max().expect("");
        let first_min = counts_first.values().filter(|x| {x != &&0}).min().expect("");
        let second_max = counts_second.values().filter(|x| {x != &&0}).max().expect("");
        let second_min = counts_second.values().filter(|x| {x != &&0}).min().expect("");

        if first_max > second_max {
            return std::cmp::Ordering::Greater;
        }
        
        if first_max < second_max {
            return std::cmp::Ordering::Less;
        }

        // full houses and two pairs need sorting before we look at card ordering
        if first_max == &3 {
            // we might have 3, 1, 1 or 3, 2
            if first_min == &2 && 
                second_min == &1 {
                return Ordering::Greater;
            }

            if first_min == &1 &&
                second_min == &2 {
                return Ordering::Less;
            }
        }

        if first_max == &2 {
            // might have 2, 1, 1, 1 or 2, 2, 1
            if counts_first.values().filter(|x| { x != &&0}).count() == 3 && counts_second.values().filter(|x| { x != &&0 }).count() == 4 {
                return Ordering::Greater;
            }
            
            if counts_second.values().filter(|x| { x != &&0}).count() == 3 && counts_first.values().filter(|x| { x != &&0 }).count() == 4 {
                return Ordering::Less;
            }
        }


        let mut first_iter = first.chars();
        let mut second_iter = second.chars();

        loop {
            let next_1 = first_iter.next().expect("should break before this is none");
            let next_2 = second_iter.next().expect("should break before this is none      2");

            if next_1 == next_2 {
                continue;
            }

            for card in cards.iter() {
                if next_1 == *card {
                    return Ordering::Greater;
                }

                if next_2 == *card {
                    return Ordering::Less;
                }
            }
        }
    });

    let mut total = 0;
    let mut count = 1;

    for hand in hands.iter() {
        total += count * hand.1;
        count += 1;
    }

    println!("{}", total);

    // part 2
    // sort, but 1. J < 2, and 2. add J to counts[max] for checking goodness of hand
    // J now last
    cards = vec!['A', 'K', 'Q', 'T', '9', '8', '7', '6', '5', '4', '3', '2', 'J'];

    hands.sort_by(|a, b| {
        let first = a.0;
        let second = b.0;

        let mut counts_first = HashMap::<char, i32>::new();
        let mut counts_second = HashMap::<char, i32>::new();

        let mut jokers_first = 0;
        let mut jokers_second = 0;

        for card in cards.iter() {
            let mut count_first = 0;
            for c in first.chars() {
                if c == *card {
                    count_first += 1;

                    if card == &'J' {
                        jokers_first+=1;
                    }
                }
            }

            counts_first.insert(*card, count_first);
            
            let mut count_second = 0;
            for c in second.chars() {
                if c == *card {
                    count_second += 1;

                    if card == &'J' {
                        jokers_second+=1;
                    }
                }
            }

            counts_second.insert(*card, count_second);
        }

        // compare on card counts
        let mut first_max: i32 = *(counts_first.values().filter(|x| {x != &&0}).max().expect(""));
        let mut first_min: i32 = *(counts_first.values().filter(|x| {x != &&0}).min().expect(""));
        let mut second_max: i32 = *(counts_second.values().filter(|x| {x != &&0}).max().expect(""));
        let mut second_min: i32 = *(counts_second.values().filter(|x| {x != &&0}).min().expect(""));

        // if the max is at `J`, then we need to find the NEXT max instead
        // otherwise just add the `J` count to the max
        first_max = match first_max {
            5 => 5,
            4 => 
                if jokers_first == 4 || jokers_first == 1 { 5 } 
                else { 4 },
            3 => 
                // either 
                // 3J 2 -> 5
                // 3 2J -> 5
                // 3 2 -> 3

                // 3J 1 1 -> 4
                // 3 1J 1 -> 4
                // 3 1 1 -> 3
                if jokers_first == 3 && first_min == 2 || jokers_first == 2 { 5 } 
                else if jokers_first == 3 && first_min == 1 || jokers_first == 1 { 4 } 
                else { 3 },
            2 =>
                // either
                // 2J 2 1 -> 4
                // 2 2 1J -> 3 && change min to 2 also 
                // 2 2 1 -> 2

                // 2J 1 1 1 -> 3
                // 2 1J 1 1 -> 3
                // 2 1 1 1 -> 2
                if jokers_first == 2 { 
                    // 2J 2 1 or 2J 1 1 1
                    if counts_first.values().filter(|x| { x != &&0}).count() == 3 { 4 } else {3}
                }
                else if jokers_first == 1 {
                    // 2 2 1J or 2 1J 1 1
                    if counts_first.values().filter(|x| { x != &&0}).count() == 3 { first_min = 2; 3} else {3}
                }
                else { first_max },
            1 => if jokers_first == 1 { 2 } else { 1 },
            _ => panic!("ohh nooooo")
        };

        // same logic, see comments above for why...
        second_max = match second_max {
            5 => 5,
            4 => 
                if jokers_second == 4 || jokers_second == 1 { 5 } 
                else { 4 },
            3 => 
                if jokers_second == 3 && second_min == 2 || jokers_second == 2 { 5 } 
                else if jokers_second == 3 && second_min == 1 || jokers_second == 1 { 4 } 
                else { 3 },
            2 =>
                if jokers_second == 2 { 
                    if counts_second.values().filter(|x| { x != &&0}).count() == 3 { 4 } else {3}
                }
                else if jokers_second == 1 {
                    if counts_second.values().filter(|x| { x != &&0}).count() == 3 { second_min = 2; 3} else {3}
                }
                else { second_max },
            1 => if jokers_second == 1 { 2 } else { 1 },
            _ => panic!("ohh nooooo")
        };

        // actually sort it 
        if first_max > second_max {
            return std::cmp::Ordering::Greater;
        }
        
        if first_max < second_max {
            return std::cmp::Ordering::Less;
        }

        // full houses and two pairs need sorting before we look at card ordering
        if first_max == 3 {
            // we might have 3, 1, 1 or 3, 2
            if first_min == 2 && 
                second_min == 1 {
                return Ordering::Greater;
            }

            if first_min == 1 &&
                second_min == 2 {
                return Ordering::Less;
            }
        }

        if first_max == 2 {
            // might have 2, 1, 1, 1 or 2, 2, 1
            // count the NUMBER of things in the hands
            if counts_first.values().filter(|x| { x != &&0}).count() == 3 && counts_second.values().filter(|x| { x != &&0 }).count() == 4 {
                return Ordering::Greater;
            }
            
            if counts_second.values().filter(|x| { x != &&0}).count() == 3 && counts_first.values().filter(|x| { x != &&0 }).count() == 4 {
                return Ordering::Less;
            }
        }

        // lastly compare by character
        let mut first_iter = first.chars();
        let mut second_iter = second.chars();

        loop {
            let next_1 = first_iter.next().expect("should break before this is none");
            let next_2 = second_iter.next().expect("should break before this is none      2");

            if next_1 == next_2 {
                continue;
            }

            for card in cards.iter() {
                if next_1 == *card {
                    return Ordering::Greater;
                }

                if next_2 == *card {
                    return Ordering::Less;
                }
            }
        }
    });

    let mut total = 0;
    let mut count = 1;

    for hand in hands.iter() {
        total += count * hand.1;
        count += 1;
    }

    println!("{}", total); // 246903088 too low
                           // 252393769 too high -- should've deleted my test code that meant this was wrong
}
