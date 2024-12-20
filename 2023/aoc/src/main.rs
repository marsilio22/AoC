use std::fs;
use std::collections::{HashMap, HashSet};
use std::cmp::Ordering;
use std::iter::zip;
use std::time::Instant;
use num::integer::lcm;
use itertools::Itertools;
use string_builder::Builder;

fn main() {
    let then = Instant::now();
    // day1();
    // day2();
    // day3();
    // day4();
    // day5();
    // day5_2();
    // day6();
    // day7();
    // day8();
    // day9();
    // day10();
    // day11();
    // day12();
    // day13();
    // day14();
    // day15();


    // day18();
    // day19();

    day21();
    let now = Instant::now();

    println!("took {:?} to run", now.duration_since(then));
}

// todo move to mod
fn day1() {
    let contents = fs::read_to_string("./inputs/day1").expect("Should have read the file");
    let list = contents.lines();

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
    let input = contents.lines();

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
    let rows = contents.lines();

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
    let rows = contents.lines();
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
    let mut rows = contents.lines();

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
    println!("{:?}", seeds[0]);
}

fn day5_2()
{
    let contents = fs::read_to_string("./inputs/day5").expect("Should have read the file");
    let mut rows = contents.lines();

    let seeds_input: Vec<i64> = rows.next().expect("should be ok")
        .split(": ").last().expect("should still be ok")
        .split(" ").map(|x| x.parse::<i64>().expect("should have i64s")).collect();

    let mut seeds: Vec<(i64, i64)> = Vec::<(i64, i64)>::new();

    for i in 0..seeds_input.len()/2 {
        seeds.push((seeds_input[2*i], seeds_input[2*i] + seeds_input[2*i+1] - 1));
    }

    rows.next();
    rows.next(); // skip past the first title;

    let mut loop_seeds = Vec::<(i64, i64)>::new();

    loop {
        let nums = match rows.next() {
            Some(a) => a,
            None => break
        };

        if nums.is_empty() {
            // replace seeds array proper for the next loop
            seeds.retain(|x| {x.0 != -1});

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
                if seed.0 >= dest_src_len[1] && seed.1 <= dest_src_len[1] + dest_src_len[2] {
                    loop_seeds.push((seed.0 - dest_src_len[1] + dest_src_len[0], seed.1 - dest_src_len[1] + dest_src_len[0]));
                    *seed = (-1, -1);
                }

                // mapping covers bottom end of seeds (split into two)
                if seed.0 >= dest_src_len[1] && seed.0 <= dest_src_len[1] + dest_src_len[2] && seed.1 >= dest_src_len[1] + dest_src_len[2] {
                    loop_seeds.push((seed.0 - dest_src_len[1] + dest_src_len[0], dest_src_len[0] + dest_src_len[2] - 1));
                    *seed = (dest_src_len[1] + dest_src_len[2], seed.1);
                }

                // mapping covers top end of seeds (split into two)
                if seed.0 < dest_src_len[1] && seed.1 >= dest_src_len[1] && seed.1 <= dest_src_len[1] + dest_src_len[2] {
                    loop_seeds.push((dest_src_len[0], dest_src_len[0] + seed.1 - dest_src_len[1]));
                    *seed = (seed.0, dest_src_len[1] - 1) // questionable minus one here
                }

                // seeds cover mapping range entirely (split into three)
                if seed.0 < dest_src_len[1] && seed.1 > dest_src_len[1] + dest_src_len[2] {
                    loop_seeds.push((dest_src_len[0], dest_src_len[0] + dest_src_len[2]));
                    *seed = (seed.0, dest_src_len[1] - 1);// lower bit
                    unmapped_seeds.push((dest_src_len[1] + dest_src_len[2] + 1, seed.1));// upper bit // dubious +1
                }
            }

            // rejoin the things together
            for thing in unmapped_seeds {
                seeds.push(thing);
            }
        }
    }

    // one final merge of seeds and loop seeds
    seeds.retain(|x| {x.0 != -1});
    for seed in loop_seeds.clone().iter() {
        seeds.push(*seed);
    }

    let mut min = i64::MAX;

    for seed in seeds.iter() {
        if seed.0 != -1 && seed.0 < min { min = seed.0 }
    }

    println!("{:?}", min); // 144724436 too high
                           // 117979283 too high
                           // 
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

        res *= max - min + 1;
    }

    println!("{}", res);
}

fn day7() {
    let contents = fs::read_to_string("./inputs/day7").expect("Should have read the file");
    let rows = contents.lines();

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

fn day8() {
    let contents = fs::read_to_string("./inputs/day8").expect("Should have read the file");
    let mut rows = contents.lines();

    let instr = rows.next().expect("yeah yeah");

    // skip blank
    rows.next();

    let mut map = HashMap::<&str, (String, String)>::new();

    // parse nodes
    loop {
        let next = match rows.next()
        {
            Some(a) => a,
            None => break
        };

        let mut split = next.split(" = ");

        let key = split.next().expect("should have 2 parts");
        let val = split.next().expect("should have 2 parts - 2");

        let replaced = val.replace(&['(', ')'][..], "");

        let split2: Vec<&str> = replaced.split(", ").collect::<Vec<&str>>();
        let tuple_value = (String::from(split2[0]), String::from(split2[1]));

        map.insert(key, tuple_value);
    }

    let mut curr = "AAA"; // p1

    let mut count = 0;

    loop {
        let mut instr_c = instr.chars();

        while curr != "ZZZ" {
            let next = instr_c.next();

            curr = match next {
                Some('L') => &map[curr].0,
                Some('R') => &map[curr].1,
                None => break,
                Some(_) => panic!("oh no!")
            };

            count += 1;
        }

        if curr == "ZZZ" {break;}
    }
    println!("{}", count);

    // p2
    
    let currents = map.keys().filter(|x| x.ends_with("A"));

    let mut counts = Vec::<i64>::new();

    for current in currents {
        let mut curr = *current;
        count = 0;

        loop {
            let mut instr_c = instr.chars();

            while !current.ends_with("Z") {
                let next = instr_c.next();

                curr = match next {
                    Some('L') => &map[curr].0,
                    Some('R') => &map[curr].1,
                    None => break,
                    Some(_) => panic!("oh no!")
                };

                count += 1;
            }

            if curr.ends_with("Z"){break;}
        }

        counts.push(count);
    }

    let mut least_common_multiple: i64 = 1;

    for count in counts {
        least_common_multiple = lcm(least_common_multiple, count);
    }

    println!("{}", least_common_multiple);
}

fn day9() {
    let contents = fs::read_to_string("./inputs/day9").expect("Should have read the file");
    let rows = contents.lines().map(|r| r.split(" ").map(|n| n.parse::<i64>().expect("should parse")));

    let mut total_last = 0;
    let mut total_first = 0;
    for row in rows {
        let readings = row.collect::<Vec<i64>>();

        let mut map = HashMap::<i32, Vec<i64>>::new();
        map.insert(0, readings);

        let mut depth = 0;
        loop {
            let vals = &map[&depth];
            let mut new_row = Vec::<i64>::new();

            for i in 0..(vals.len()-1)
            {
                new_row.push(vals[i+1] - vals[i]);
            }

            depth += 1;
            map.insert(depth, new_row.clone());

            if new_row.iter().all(|f| f == &0) {
                break;
            }
        }

        let mut subtotal_last = 0;
        for reading in map.clone() {
            subtotal_last += reading.1.last().expect("should have at least one entry");
        }

        total_last += subtotal_last;

        //p2 
        let mut subtotal_first = 0;

        depth -= 1;
        loop {
            subtotal_first = map[&depth].first().expect("should have at least one value") - subtotal_first;
            depth -= 1;

            if depth == -1 { break }
        }
        total_first += subtotal_first;
    }

    println!("{}", total_last);
    println!("{}", total_first); 
}

fn day10() {
    let contents = fs::read_to_string("./inputs/day10").expect("Should have read the file");
    let rows = contents.lines();

    let mut map = HashMap::<(i32, i32), char>::new();

    for (j, r) in rows.enumerate() {
        for (i, c) in r.chars().enumerate() {
            map.insert((i.try_into().unwrap(), j.try_into().unwrap()), c);
        }
    }

    let clone = map.clone();
    let start = clone.iter().filter(|x| x.1 == &'S').collect::<Vec<(&(i32, i32),&char)>>()[0].0;

    let mut current = *start;
    let mut last = *start;

    let mut length = 0;

    // by observation, S is an F, todo put this in code using the below...
    let mut curr_pipe = 'F';

    // let valid_north_chars = ['F', '|', '7'];
    // let valid_east_chars = ['J', '-', '7'];
    // let valid_south_chars = ['J', '|', 'L'];
    // let valid_west_chars = ['F', '-', 'L'];

    // if valid_east_chars.contains(&map[&dirs[0]]) && dirs[0] != last {
    //     last = current;
    //     current = dirs[0];
    // }
    // else if valid_west_chars.contains(&map[&dirs[1]]) && dirs[1] != last {
    //     last = current;
    //     current = dirs[1];
    // }
    // else if valid_south_chars.contains(&map[&dirs[2]]) && dirs[2] != last {
    //     last = current;
    //     current = dirs[2];
    // }
    // else if valid_north_chars.contains(&map[&dirs[3]]) && dirs[3] != last {
    //     last = current;
    //     current = dirs[3];
    // }

    let mut whole_loop = HashMap::<(i32, i32), char>::new();
    
    loop {
        let east = (current.0 + 1, current.1); //East
        let west = (current.0 - 1, current.1); //West
        let south = (current.0, current.1 + 1); //South
        let north = (current.0, current.1 - 1); //North
        

        let valid_dirs = match curr_pipe {
            '|' => [south, north],
            '-' => [east, west],
            'L' => [east, north],
            'J' => [west, north],
            '7' => [west, south],
            'F' => [east, south],
            'S' => break,
            _ => panic!("oh noes")
        };

        let next = valid_dirs.iter().filter(|x| x != &&last).next().unwrap();

        let mut insides = Vec::<(i32, i32)>::new();
        let mut outsides = Vec::<(i32,i32)>::new();
        let mut to_insert = '.';

        if *next == east {
            to_insert = '>';
            // north is INSIDE
            insides.push((last.0, last.1 - 1));
            insides.push((current.0, current.1 - 1));
            insides.push((next.0, next.1 - 1));
            // south is OUTSIDE;
            outsides.push((last.0, last.1 + 1));
            outsides.push((current.0, current.1 + 1));
            outsides.push((next.0, next.1 + 1));
        }
        if *next == west {
            to_insert = '<';
            // north is OUTSIDE
            outsides.push((last.0, last.1 - 1));
            outsides.push((current.0, current.1 - 1));
            outsides.push((next.0, next.1 - 1));
            // south is INSIDE
            insides.push((last.0, last.1 + 1));
            insides.push((current.0, current.1 + 1));
            insides.push((next.0, next.1 + 1));
        }
        if *next == north {
            to_insert = '^';
            // east is OUTSIDE
            outsides.push((last.0 + 1, last.1));
            outsides.push((current.0 + 1, current.1));
            outsides.push((next.0 + 1, next.1));
            // west is INSIDE
            insides.push((last.0 - 1, last.1));
            insides.push((current.0 - 1, current.1));
            insides.push((next.0 - 1, next.1));
        }
        if *next == south {
            to_insert = 'v';
            // east is INSIDE
            insides.push((last.0 + 1, last.1));
            insides.push((current.0 + 1, current.1));
            insides.push((next.0 + 1, next.1));
            // west is OUTSIDE
            outsides.push((last.0 - 1, last.1));
            outsides.push((current.0 - 1, current.1));
            outsides.push((next.0 - 1, next.1));
        }

        // wtf have I done wrong here
        // let to_insert: char = match *next {
        //     east => '>',
        //     west => '<',
        //     south => 'v',
        //     north => '^',
        //     _ => panic!("should've been ONE of those...")
        // };

        whole_loop.insert(current, to_insert);
        for inside in insides {
            if map.contains_key(&inside) && !whole_loop.contains_key(&inside) { whole_loop.insert(inside, 'I'); }
        }
        for outside in outsides {
            if map.contains_key(&outside) && !whole_loop.contains_key(&outside){ whole_loop.insert(outside, 'O'); }
        }

        last = current;
        current = *next;
        curr_pipe = map[&current];

        length += 1;
    }

    println!("{}", length/2);

    // I originally solved this by just looking at the picture, and using Ctrl+F to count I's and blanks, but now the code below does that :)
    // for j in 0..140 {
    //     for i in 0..140 {
    //         if whole_loop.contains_key(&(i, j)) {
    //             if map[&(i, j)] == 'S' { print!("S")}
    //             else{print!("{}", whole_loop[&(i, j)])}
    //         }
    //         else { print!(" ")}
    //     }
    //     println!();
    // }

    let mut min_inside_x = i32::MAX;
    let mut max_inside_x = 0;
    let mut min_inside_y = i32::MAX;
    let mut max_inside_y = 0;

    let mut inside_spaces = 0;

    for loop_entry in whole_loop.clone() {
        if loop_entry.1 == 'I' {
            inside_spaces += 1;

            if loop_entry.0.0 < min_inside_x { min_inside_x = loop_entry.0.0 };
            if loop_entry.0.0 > max_inside_x { max_inside_x = loop_entry.0.0 };
            if loop_entry.0.1 < min_inside_y { min_inside_y = loop_entry.0.1 };
            if loop_entry.0.1 > max_inside_y { max_inside_y = loop_entry.0.1 };
        }
    }

    let mut undocumented_inside_spaces = 0;

    for j in min_inside_y..max_inside_y {
        for i in min_inside_x..max_inside_x {
            if !whole_loop.contains_key(&(i, j))
            {
                undocumented_inside_spaces += 1;
            }
        }
    }

    println!("{}", undocumented_inside_spaces + inside_spaces);
}

fn day11() {
    let contents = fs::read_to_string("./inputs/day11").expect("Should have read the file");
    let rows = contents.lines();

    let mut map = HashMap::<(i32, i32), char>::new();

    for (j, r) in rows.enumerate() {
        for (i, c) in r.chars().enumerate() {
            if c == '#' {
                map.insert((i.try_into().unwrap(), j.try_into().unwrap()), c);
            }
        }
    }

    let mut columns_to_add = Vec::<i32>::new();
    let mut rows_to_add = Vec::<i32>::new();

    for i in 0..140 {
        if map.keys().all(|k| k.0 != i) {
            columns_to_add.push(i);
        }
        if map.keys().all(|k| k.1 != i) {
            rows_to_add.push(i);
        }
    }

    // NB for part 2 we don't add 1million here, because we already have 1, so it's increase TO 1mill, not increas BY 1mill
    for k in [1, 999999] {
        let mut expanded_map = HashMap::<(i32, i32), char>::new();

        for galaxy in map.clone() {
            let x_to_add: i32 = columns_to_add.iter().filter(|c| c < &&(galaxy.0.0)).count().try_into().unwrap();
            let y_to_add: i32 = rows_to_add.iter().filter(|r| r < &&(galaxy.0.1)).count().try_into().unwrap();

            expanded_map.insert((galaxy.0.0 + x_to_add*k, galaxy.0.1 + y_to_add*k), galaxy.1);
        }

        let mut total:i64 = 0;

        for galaxy in expanded_map.clone() {
            // work out min distance to each other galaxy 
            for galaxy2 in expanded_map.clone() {
                //NB we do count the distance between each galaxy and itself, which is 0, so adds nothing to the total
                total += i64::from((galaxy2.0.0 - galaxy.0.0).abs() + (galaxy2.0.1 - galaxy.0.1).abs());
            }
        }

        println!("{}", total/2);
    }
}

fn day12() {
    let contents = fs::read_to_string("./inputs/day12").expect("Should have read the file");
    let rows = contents.lines();

    // number of groups of filled in elements + number of spaces NOT between them
    // e.g. fitting 1, 1, 1 into 10 spaces is 3 groups, and 4 spaces = choice of 1 to 7

    // tuple length is number of groups
    let mut total = 0;
    for row in rows.clone() {
        let mut options = 0;

        let mut split = row.split(" ");

        let pattern = split.next().expect("should have pattern");
        let groups_str = split.next().expect("should have groups");

        let groups = groups_str.split(",")
            .map(|x| x.parse::<i32>().expect("groups should be i32"))
            .collect::<Vec<i32>>();

        // max group length is 6
        // white spaces remaining is total spaces - (group sum + group length - 1)

        let groups_len:i32 = groups.len().try_into().unwrap();
        let to_subtract:i32 = groups.iter().sum::<i32>() + groups_len - 1;

        let pattern_len:i32 = pattern.len().try_into().unwrap();

        let white_spaces = pattern_len - to_subtract;

        let iter_range = 0..(groups_len + white_spaces);

        // println!("{} - {} = {:?}", pattern_len, to_subtract, iter_range);
        let combos = iter_range.clone().combinations(groups.len()).collect::<Vec<Vec<i32>>>();

        for combo in combos {
            let mut builder = Builder::default();
            for j in 0..combo.len() {
                let dot_count = if j == 0 { combo[0] } else { combo[j] - combo[j-1] };
                for _ in 0..dot_count {
                    builder.append(".");
                }
                // groups and combos contain the same number of elements, I hope
                for _ in 0..groups[j] {
                    builder.append("#");
                }
            }

            for _ in 0..(pattern_len - (groups.iter().sum::<i32>() + combo.last().expect("should have last"))) {
                builder.append(".");
            }

            let mut valid = true;
            for pair in zip(builder.string().unwrap().chars(), pattern.chars()) {
                if pair.1 != '?' && pair.0 != pair.1 {
                    valid = false;
                    break;
                }
            }

            options += if valid { 1 } else { 0 }
        }

        total += options;
    }

    println!("{}", total);




    total = 0;
    for row in rows.clone() {
        let mut options = 0;

        let mut split = row.split(" ");

        let mut pattern_builder = Builder::default();
        let mut groups_builder = Builder::default();

        let pattern = split.next().expect("should have pattern");
        let groups_str = split.next().expect("should have groups");

        pattern_builder.append(pattern);
        groups_builder.append(groups_str);

        for _ in 0..4 {
            pattern_builder.append("?");
            pattern_builder.append(pattern);
            groups_builder.append(",");
            groups_builder.append(groups_str);
        }

        let pattern = pattern_builder.string().expect("");
        let groups_str = groups_builder.string().expect("");

        let groups = groups_str.split(",")
            .map(|x| x.parse::<i32>().expect("groups should be i32"))
            .collect::<Vec<i32>>();

        let groups_len:i32 = groups.len().try_into().unwrap();
        let to_subtract:i32 = groups.iter().sum::<i32>() + groups_len - 1;

        let pattern_len:i32 = pattern.len().try_into().unwrap();

        let white_spaces = pattern_len - to_subtract;

        let iter_range = 0..(groups_len + white_spaces);
        // this line tanks the memory creating 70k TB of combos...
        let combos = iter_range.clone().combinations(groups.len()).collect::<Vec<Vec<i32>>>();

        for combo in combos {
            let mut builder = Builder::default();
            for j in 0..combo.len() {
                let dot_count = if j == 0 { combo[0] } else { combo[j] - combo[j-1] };
                for _ in 0..dot_count {
                    builder.append(".");
                }
                // groups and combos contain the same number of elements, I hope
                for _ in 0..groups[j] {
                    builder.append("#");
                }
            }

            for _ in 0..(pattern_len - (groups.iter().sum::<i32>() + combo.last().expect("should have last"))) {
                builder.append(".");
            }

            let mut valid = true;
            for pair in zip(builder.string().unwrap().chars(), pattern.chars()) {
                if pair.1 != '?' && pair.0 != pair.1 {
                    valid = false;
                    break;
                }
            }

            options += if valid { 1 } else { 0 }
        }

        total += options;
    }

    println!("{}", total);
}

fn day13() {
    let contents = fs::read_to_string("./inputs/day13").expect("Should have read the file");
    let mut rows = contents.lines(); 

    let mut maps = Vec::<Vec<&str>>::new();

    let mut current = vec![];

    let mut total = HashMap::<i32, i32>::new(); // map index to score
    let mut index = 0;
    
    loop { 
        let row = match rows.next() {
            Some(a) => a,
            None => break
        };

        if row.is_empty() {
            maps.push(current);
            current = vec![];
            continue;
        }

        current.push(row);
    }

    // we never add the last map, because it breaks before it gets there. Hack
    maps.push(current);

    for map in maps {
        // check rows
        let mut it_was_a_row = false;

        println!("{:?}", map);

        for row in map.clone().iter().enumerate() {
            if map.len() > row.0 + 1 {
                // this isn't yet the last row, so check whether this matches the *next* row
                if row.1.eq(&map[row.0 + 1]) {
                    // now we need to check every surrounding pair for equality, until we fall off one way or the other
                    let set_1 = map.iter().take(row.0 + 1).rev();
                    let set_2 = map.iter().skip(row.0 + 1);

                    // zip works how we want it to, and discards tuples where either iterator was None
                    let mut valid = true;
                    for pair in set_1.zip(set_2) {

                        println!("pair = {:?}", pair);

                        if pair.0 != pair.1 {
                            valid = false;
                            break;
                        }
                    }

                    if valid == true {
                        total.insert(index, (100 * (row.0 + 1)).try_into().unwrap());
                        it_was_a_row = true;
                        index += 1;
                        break;
                    }
                }
            }
        }

        if it_was_a_row { continue; }

        // check cols

        let mut strb = Builder::default();

        for i in 0..map[0].len() {
            for row in map.clone().iter().rev() {
                strb.append(row.chars().skip(i).next().expect(""));
            }
            strb.append("-");
        }

        // hack around string references in loop
        let constructed_string = strb.string().expect("");
        let new_rows = constructed_string.split("-").filter(|x| !x.is_empty()).collect::<Vec<&str>>();

        // literally copy paste of the above logic for rows, again
        println!("{:?}", new_rows);
        for row in new_rows.iter().enumerate() {
            if new_rows.len() > row.0 + 1 {
                // this isn't yet the last row, so check whether this matches the *next* row
                if row.1.eq(&new_rows[row.0 + 1]) {
                    // now we need to check every surrounding pair for equality, until we fall off one way or the other
                    let set_1 = new_rows.iter().take(row.0 + 1).rev();
                    let set_2 = new_rows.iter().skip(row.0 + 1);

                    // zip works how we want it to, and discards tuples where either iterator was None
                    let mut valid = true;
                    for pair in set_1.zip(set_2) {

                        println!("pair = {:?}", pair);

                        if pair.0 != pair.1 {
                            valid = false;
                            break;
                        }
                    }

                    if valid == true { 
                        total.insert(index, (row.0 + 1).try_into().unwrap());
                        index += 1;
                        break; 
                    }
                }
            }
        }
    }

    println!("{}", total.values().sum::<i32>());


    // now for each map we want to:
    // - change a single entry, 
    // - check the score, and 
    // - *change* the value in `total` if and only if the new score is:
    //   - *different*, and 
    //   - *non-zero*

    // NB ALSO need to find ALL lines, and only pick the ones that are different, instead of failing fast
    // this is probably a good place to start, editing p1 to do this 

    //p2 25676 too low 
}

fn day14() {
    let contents = fs::read_to_string("./inputs/day14").expect("Should have read the file");
    let rows = contents.lines();

    let mut map = HashMap::<(usize, usize), char>::new();
    let mut max_j = 0;
    for (j, r) in rows.clone().enumerate() {
        for (i, c) in r.chars().enumerate() {
            if c != '.' {
                // hack this to start at index 1 for reasons of usize being unsigned, and needingto denote the top of the map later
                map.insert(((i+1).try_into().unwrap(), (j+1).try_into().unwrap()), c);
            }
        }
        max_j = j + 1; // fml
    }

    let mut new_map = day14_move_north(max_j+1, map);

    let mut total = 0;

    for thing in new_map.clone() {
        if thing.1 == 'O' {
            total += (max_j - thing.0.1 + 1);
        }
    }

    println!("{}", total);

    //p2
    let mut maps_over_time = HashMap::<i32, HashMap<(usize, usize), char>>::new();
    
    new_map = day14_move_west(max_j+1, new_map);
    new_map = day14_move_south(max_j+1, new_map);
    new_map = day14_move_east(max_j+1, new_map);

    let mut i = 1;

    maps_over_time.insert(i, new_map.clone());

    // this takes about 7.5 minutes to run...
    loop {
        i += 1;
        
        new_map = day14_move_north(max_j + 1, new_map);
        new_map = day14_move_west(max_j+1, new_map);
        new_map = day14_move_south(max_j+1, new_map);
        new_map = day14_move_east(max_j+1, new_map);

        let clone = maps_over_time.clone();
        let dupes = clone.iter().filter(|m| m.1 == &new_map);

        if dupes.clone().count() > 0 {
            let dupe = dupes.last().expect("");

            let state = ((1_000_000_000 - dupe.0) % (i - dupe.0)) + dupe.0;
            let mut total = 0;

            for thing in maps_over_time[&state].clone() {
                if thing.1 == 'O' {
                    total += (max_j - thing.0.1 + 1);
                }
            }

            println!("{}", total);
            break;
        }
        else {
            maps_over_time.insert(i, new_map.clone());
        }
    }
}

fn day14_move_north(row_count: usize, map: HashMap<(usize, usize), char>) -> HashMap<(usize, usize), char>
{
    let mut new_map = HashMap::<(usize, usize), char>::new();
    
    for j in 0..row_count {
        // for each column
        // move every `O` to the lowest down `#` north of it plus the number of `O`'s north of it

        let map_clone = map.clone();
        let column = map_clone.iter().filter(|z| z.0.0 == j);

        for entry in column.clone() {
            if entry.1 == &'O' {
                let closest_rock_north = match column.clone()
                    .filter(|x| x.1 == &'#' && x.0.1 < entry.0.1)
                    .max_by_key(|x| x.0.1 ) {
                        Some(a) => (*a.0, a.1) ,
                        None => {let key = (entry.0.0, 0); (key , &'#')},
                    };

                // if entry.0.0 == 0 {
                //     println!("{:?} -> {:?}", entry, closest_rock_north);
                // }

                let number_of_round_rocks_north = 
                    column.clone().filter(|c| c.1 == &'O' && c.0.1 > closest_rock_north.0.1 && c.0.1 < entry.0.1 ).count();

                let new_key = (closest_rock_north.0.0, closest_rock_north.0.1 + number_of_round_rocks_north + 1);

                new_map.insert(new_key, 'O');
            }
            else {
                new_map.insert(*entry.0, *entry.1);
            }
        }
    }

    return new_map;
}

fn day14_move_west(row_count: usize, map: HashMap<(usize, usize), char>) -> HashMap<(usize, usize), char>
{
    let mut new_map = HashMap::<(usize, usize), char>::new();
    
    for j in 0..row_count {
        let map_clone = map.clone();
        let row = map_clone.iter().filter(|z| z.0.1 == j);

        for entry in row.clone() {
            if entry.1 == &'O' {
                let closest_rock_west = match row.clone()
                    .filter(|x| x.1 == &'#' && x.0.0 < entry.0.0)
                    .max_by_key(|x| x.0.0 ) {
                        Some(a) => (*a.0, a.1) ,
                        None => {let key = (0, entry.0.1); (key , &'#')},
                    };

                // if entry.0.0 == 0 {
                //     println!("{:?} -> {:?}", entry, closest_rock_north);
                // }

                let number_of_round_rocks_west = 
                    row.clone().filter(|c| c.1 == &'O' && c.0.0 > closest_rock_west.0.0 && c.0.0 < entry.0.0 ).count();

                let new_key = (closest_rock_west.0.0 + number_of_round_rocks_west + 1, closest_rock_west.0.1);

                new_map.insert(new_key, 'O');
            }
            else {
                new_map.insert(*entry.0, *entry.1);
            }
        }
    }

    return new_map;
}

fn day14_move_south(row_count: usize, map: HashMap<(usize, usize), char>) -> HashMap<(usize, usize), char>
{
    let mut new_map = HashMap::<(usize, usize), char>::new();
    
    for j in 0..row_count {
        let map_clone = map.clone();
        let column = map_clone.iter().filter(|z| z.0.0 == j);

        for entry in column.clone() {
            if entry.1 == &'O' {
                let closest_rock_south = match column.clone()
                    .filter(|x| x.1 == &'#' && x.0.1 > entry.0.1)
                    .min_by_key(|x| x.0.1 ) {
                        Some(a) => (*a.0, a.1) ,
                        None => {let key = (entry.0.0, row_count); (key , &'#')}, // maybe off by one
                    };

                // if entry.0.0 == 0 {
                //     println!("{:?} -> {:?}", entry, closest_rock_north);
                // }

                let number_of_round_rocks_south = 
                    column.clone().filter(|c| c.1 == &'O' && c.0.1 < closest_rock_south.0.1 && c.0.1 > entry.0.1 ).count();

                let new_key = (closest_rock_south.0.0, closest_rock_south.0.1 - number_of_round_rocks_south - 1);

                new_map.insert(new_key, 'O');
            }
            else {
                new_map.insert(*entry.0, *entry.1);
            }
        }
    }

    return new_map;
}

fn day14_move_east(row_count: usize, map: HashMap<(usize, usize), char>) -> HashMap<(usize, usize), char>
{
    let mut new_map = HashMap::<(usize, usize), char>::new();
    
    for j in 0..row_count {
        let map_clone = map.clone();
        let row = map_clone.iter().filter(|z| z.0.1 == j);

        for entry in row.clone() {
            if entry.1 == &'O' {
                let closest_rock_east = match row.clone()
                    .filter(|x| x.1 == &'#' && x.0.0 > entry.0.0)
                    .min_by_key(|x| x.0.0 ) {
                        Some(a) => (*a.0, a.1) ,
                        None => {let key = (row_count, entry.0.1); (key , &'#')},
                    };

                // if entry.0.0 == 0 {
                //     println!("{:?} -> {:?}", entry, closest_rock_north);
                // }

                let number_of_round_rocks_east = 
                    row.clone().filter(|c| c.1 == &'O' && c.0.0 < closest_rock_east.0.0 && c.0.0 > entry.0.0 ).count();

                let new_key = (closest_rock_east.0.0 - number_of_round_rocks_east - 1, closest_rock_east.0.1);

                new_map.insert(new_key, 'O');
            }
            else {
                new_map.insert(*entry.0, *entry.1);
            }
        }
    }

    return new_map;
}

fn day15() {
    let contents = fs::read_to_string("./inputs/day15").expect("Should have read the file");
    
    let items = contents.lines().last().expect("only one line in this one").split(",");

    let mut total = 0;

    for item in items.clone() {
        total += day15_hash(item);
    }

    println!("{:?}", total);

    let mut boxes = HashMap::<u32, HashMap<&str, (i32, usize)>>::new(); // lens then order of insertion

    for item in items.clone().enumerate() {
        if item.1.contains("=") {
            let mut split = item.1.split("=");
            let label = split.next().expect("");
            let num = split.next().expect("").parse::<i32>().expect("");

            boxes
                .entry(day15_hash(label))
                .and_modify(|inner_map| {
                    // why is this seem to be necessary...
                    // why does this not work: inner_map.entry(label).and_modify(|inner_lens| { (num, inner_lens.1) }).or_insert((num, item.0));
                    // something to do with tuples and immutability??
                    if (inner_map.contains_key(label))
                    {
                        inner_map.insert(label, (num, inner_map[label].1));
                    }
                    else {
                        inner_map.insert(label, (num, item.0)); 
                        
                    }
                })
                .or_insert(HashMap::<&str, (i32, usize)>::from([(label, (num, item.0))]));
        }
        else { 
            let mut split = item.1.split("-");

            let label = split.next().expect("");

            boxes.entry(day15_hash(label)).and_modify(|inner_map| { inner_map.remove(label); });
        }
    }

    let total = 
        boxes.clone().iter().map(|x| {
            x.1.values()
                .sorted_by(|a, b| a.1.cmp(&b.1))
                .enumerate()
                .map(|z| {(z.0 + 1) * z.1.0 as usize * (*x.0 as usize + 1)})
                .sum::<usize>()
        }).sum::<usize>();


    println!("{}", total);
}

fn day15_hash (item: &str) -> u32 {
    let mut total = 0;

    for c in item.chars() {
        total += c as u32;
        total *= 17;
        total %= 256;
    }

    return total;
}

fn day16() {
    let contents = fs::read_to_string("./inputs/day15").expect("Should have read the file");
    let rows = contents.lines();

    let mut map = HashMap::<(i32, i32), char>::new();

    for (j, r) in rows.enumerate() {
        for (i, c) in r.chars().enumerate() {
            if c != '.' {
                map.insert((i.try_into().unwrap(), j.try_into().unwrap()), c);
            }
        }
    }

    let mut energize_map = HashSet::<(i32, i32)>::new();

    let curr = (-1, 0);
    let dir = '>';

    let mut to_calc = Vec::<((i32, i32), char)>::new();

    to_calc.push((curr, dir));

    while !to_calc.is_empty()
    {
        let this_round = to_calc.pop().expect("already checked for emptiness"); // could probably move the assignment and check to the while def but wevs

        // follow this thing to its endpoint, adding splits to the to_calc as we go
        loop {
            let mut dir = match this_round.1  {
                '>' => (1, 0),
                '<' => (-1, 0),
                '^' => (0, -1),
                'v' => (0, 1),
                _ => panic!("jwjf")
            };

            let mut next = (this_round.0.0 + dir.0, this_round.0.1 + dir.1);

            // exit if this falls off the map?
            while !map.contains_key(&next) {
                energize_map.insert(next);

                next = (next.0 + dir.0, next.1 + dir.1);
            }

            // we've found something in the map, decide what to do
        }
    }
}

fn day18() {
    let contents = fs::read_to_string("./inputs/day18").expect("Should have read the file");
    let rows = contents.lines();

    let mut map = HashMap::<usize, (i32, i32)>::new();
    let mut map_p2 = HashMap::<usize, (i32, i32)>::new();

    let mut current = (0, 0);
    let mut current_p2 = (0, 0);

    map.insert(0, current);
    map_p2.insert(0, current_p2);

    let mut perimeter = 0;
    let mut perimeter_p2 = 0;
    for row in rows.clone().enumerate() {
        let mut split = row.1.split(" ");

        let dir = split.next().expect("");
        let distance = split.next().expect("").parse::<i32>().expect("");

        perimeter += distance;

        let direction= match dir {
            "U" => (0, -1),
            "D" => (0, 1),
            "L" => (-1, 0),
            "R" => (1, 0),
            _ => panic!()
        };

        let next = (current.0 + distance*direction.0, current.1 + distance*direction.1);

        map.insert(row.0 + 1, next);
        
        current = next;

        let colour = split.next().expect("").split("(#").last().expect("").split(")").next().expect("");

        let distance_p2 = i32::from_str_radix(&colour[..5], 16).expect("");
        let direction_p2 = if colour.ends_with("0") {(1, 0)} else if colour.ends_with("1") {(0, 1)} else if colour.ends_with("2") {(-1, 0)} else {(0, -1)};

        perimeter_p2 += distance_p2;

        let next_p2 = (current_p2.0 + distance_p2 * direction_p2.0, current_p2.1 + distance_p2 * direction_p2.1);

        map_p2.insert(row.0 + 1, next_p2);
        current_p2 = next_p2;
    }

    let mut total = 0;
    let mut total_p2: i64 = 0;
    
    for i in 0.. rows.clone().count() + 1 {
        let x0y0 = map[&i];
        let x1y1 = if map.contains_key(&(i+1)) { map[&(i+1)]} else { map[&0] };
        total += x0y0.0 * x1y1.1 - x0y0.1 * x1y1.0;

        let x0y0_p2 = map_p2[&i];
        let x1y1_p2 = if map_p2.contains_key(&(i+1)) { map_p2[&(i+1)]} else { map_p2[&0] };
        total_p2 += i64::from(x0y0_p2.0) * i64::from(x1y1_p2.1) - i64::from(x0y0_p2.1) * i64::from(x1y1_p2.0);
    }
    
    println!("{}", (total + perimeter)/2 + 1);
    println!("{}", (total_p2 + i64::from(perimeter_p2))/2 + 1);
}

fn day19() {
    let contents = fs::read_to_string("./inputs/day18").expect("Should have read the file");
    let mut rows = contents.lines();

    loop {
        let row = rows.next().expect("");
        if row.is_empty() { break; }

        // split by { to get the name of the thing
        // split by , to split up the instructions
        // split by : to determine where pieces go based on the test with special cases for R and A
        // check for < > , x m a s, etc to see what the test actually is

        // make a dict by name, of the tests (as functions of tuples? as functions of a class?)
    }
}

fn day21() {
        let contents = fs::read_to_string("./inputs/day21").expect("Should have read the file");
    let rows = contents.lines();

    let mut map = HashMap::<(i32, i32), char>::new();

    let mut start = (0, 0);

    for (j, r) in rows.clone().enumerate() {
        for (i, c) in r.chars().enumerate() {
            if c == '#' { 
                continue;
            }

            if c == 'S' { 
                start = (i.try_into().unwrap(), j.try_into().unwrap());
                map.insert(start, '.');
            }
            else {
                map.insert((i.try_into().unwrap(), j.try_into().unwrap()), c);
            }
        }
    }

    let mut distances = HashMap::<(i32, i32), i32>::new();

    distances.insert(start, 0);

    for i in 0..64 {
        for coord in distances.clone() {
            if i == 0 || coord.1 == i {
                for thing in vec![(0, 1), (0, -1), (1, 0), (-1, 0)]
                {
                    let key = (coord.0.0 + thing.0, coord.0.1 + thing.1);
                    if !map.contains_key(&key) {continue;}
                    
                    if !distances.contains_key(&key) {
                        distances.insert(key, i+1);
                    }
                }
            }
        }
    }

    let count = distances.clone().iter().filter(|x| x.1 % 2 == 0).count();

    println!("{}", count);

    let mut i = 64;
    let mut last_loop = 1;
    
    while distances.clone().iter().count() > last_loop { 
        last_loop = distances.clone().iter().count();

        for coord in distances.clone() {
            if coord.1 == i {
                for thing in vec![(0, 1), (0, -1), (1, 0), (-1, 0)]
                {
                    let key = (coord.0.0 + thing.0, coord.0.1 + thing.1);
                    if !map.contains_key(&key) {continue;}
                    
                    if !distances.contains_key(&key) {
                        distances.insert(key, i+1);
                    }
                }
            }
        }

        i += 1;
    }
    
    let mut total = 0;
    let target: i128 = 26501365 / 131; // 202299

    let even = distances.clone().iter().filter(|x| x.1 % 2 == 0).count() as i128;
    let odd = distances.clone().iter().filter(|x| x.1 % 2 == 1).count() as i128;

    // this just skips iterating odd then even and adding 4*even or 4*odd to each thing, for i in 1..target ...
    // notice that the numbers in the odd/even columns below are just squares
    /*
        |    n | odd | even |
        |    0 |   1 |    0 |
        |    1 |   1 |    4 |
        |    2 |   9 |    4 |
        |    3 |   9 |   16 |
        ...
     */

    // so even is n^2 and odd is (n+1)^ 2 (because n is 202300, which itself is even)
    total += target * target * even;
    total += (target + 1) * (target + 1) * odd; 

    // now need to sort out the edge triangles 
    let corners = distances.clone().iter().filter(|x| x.1 > &65).map(|x| (*x.0, *x.1)).collect::<HashMap<(i32, i32), i32>>();

    // there are n squares where we haven't yet accounted for an additional corner, and n+1 squares where we have overcounted a corner (actually there are 4n and 4(n+1) quarter squares but wevs)
    // so remove those.
    total += target * corners.clone().iter().filter(|x| x.1 % 2 == 0).count() as i128;
    total -= (target + 1) * corners.clone().iter().filter(|x| x.1 % 2 == 1).count() as i128;

    println!("{}", total);  
}

fn day22() {
    let contents = fs::read_to_string("./inputs/day22").expect("Should have read the file");
    let rows = contents.lines();

    let mut map = HashSet::<(i32, i32, i32)>::new();

    for line in rows.clone() {
        let mut split = line.split("~");

        let start = split.next().expect("").split(",").map(|x| x.parse::<i32>().expect(""));
        let end = split.next().expect("").split(",").map(|x| x.parse::<i32>().expect(""));

        
    }
}